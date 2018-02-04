using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityUtility;
using Object = UnityEngine.Object;

namespace WeaponAssemblage
{
	[CustomEditor(typeof(MonoPart), true)]
	public class MonoPartEditor : Editor
	{
		MonoPart part;
		MonoPort rootPort;
		MonoPort selectedPort;
		SerializedObject serializedPort;
		string portAcceptedType = "";

		readonly static Color kPortColor = new Color(1, 0.92f, 0.016f, 0.5f);
		readonly static Color kRootPortColor = new Color(1, 0, 0, 0.5f);
		readonly static float kPortSize = 0.04f;
		readonly static float kPortPickSize = 0.045f;
		

		private void OnEnable()
		{
			part = target as MonoPart;

			if (part.RootPort.IsExists())
			{
				rootPort = (MonoPort)part.RootPort;
			}
			else // 如果该部件没有根接口..
			{
				Debug.Log("Adding root port");
				// 尝试在其子对象中寻找根接口
				rootPort = part.transform.Find("RootPort")?.GetComponent<MonoPort>();

				// 如果没有..
				if (rootPort == null)
				{
					// 新创建一个
					rootPort = new GameObject("RootPort").AddComponent<BasicPort>();
					rootPort.transform.parent = part.transform;
					rootPort.transform.localPosition = Vector3.zero;
				}

				// 将根部件加入到部件上
				part.AddPort(rootPort, true);
			}
		}

		private void OnSceneGUI()
		{
			var ports = part.Ports;

			DrawPort(rootPort);
			foreach (MonoPort p in ports)
			{
				DrawPort(p);
			}
		}

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			if (GUILayout.Button("添加接口"))
			{
				var count = part.PortCount;
				var port = new GameObject($"port{count}").AddComponent<BasicPort>();
				port.transform.parent = part.transform;
				port.transform.localPosition = Vector3.zero;
				if (!part.AddPort(port))
				{
					Debug.LogWarning("添加新接口失败！");
					DestroyImmediate(port.gameObject);
				}
			}

			if (selectedPort != null && selectedPort.IsExists())
			{
				if (GUILayout.Button("移除接口"))
				{
					if (!part.RemovePort(selectedPort))
					{
						Debug.LogWarning("移除接口失败！");
					}
					else
					{
						DestroyImmediate(selectedPort);
						selectedPort = null;
					}
				}
				else
				{
					if (!serializedPort.IsExists())
						GetSelectedPortInfo();
					DrawSelectedPortInspector();
				}
			}
		}

		/// <summary>
		/// 获取端口信息
		/// </summary>
		void GetSelectedPortInfo()
		{
			serializedPort = new SerializedObject(selectedPort);

			// 获取端口适用类型并转换为文字
			portAcceptedType = "";
			var i = 0;
			foreach (bool b in selectedPort.SuitableType)
			{
				if (b)
				{
					portAcceptedType += "\n";
					portAcceptedType += ((PartType)i);
				}
				i++;
			}

			// 获取端口位置
			var position = selectedPort.transform.localPosition;
			position.z = selectedPort.transform.eulerAngles.z;
			selectedPort.Position = position;
		}

		/// <summary>
		/// 绘制端口
		/// </summary>
		/// <param name="port"></param>
		void DrawPort(MonoPort port)
		{
			if (port == null) return;
			if (port == rootPort) Handles.color = kRootPortColor;
			else Handles.color = kPortColor;
			Vector3 position = port.transform.position;
			//float size = HandleUtility.GetHandleSize(position);

			DrawDirectionArrow(position, port.transform.rotation, 0.1f);
			if (Handles.Button(position, Quaternion.identity, kPortSize, kPortPickSize, Handles.DotHandleCap))
			{
				selectedPort = port;
				GetSelectedPortInfo();
				Repaint();
			}

			if (selectedPort == port)
			{
				var rotation = port.transform.rotation;
				
				Handles.Label(position + new Vector3(-0.05f, 0.05f), $"接口名称：{port.name}\n可接纳部件类型：{portAcceptedType}");

				EditorGUI.BeginChangeCheck();
				rotation = Handles.DoRotationHandle(rotation, position);
				position = Handles.FreeMoveHandle(position, rotation, kPortPickSize, Vector3.one * 0.1f, Handles.DotHandleCap);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(port, "Move port");
					EditorUtility.SetDirty(port);
					port.transform.position = position;
					port.transform.rotation = rotation;
					position = part.transform.InverseTransformPoint(position);
					position.z = rotation.eulerAngles.z;
					port.Position = position;
					Repaint();
				}
			}
		}

		/// <summary>
		/// 如果选中一个端口，显示端口信息
		/// </summary>
		void DrawSelectedPortInspector()
		{
			// 显示端口名称
			GUILayout.BeginHorizontal();
			GUILayout.Label($"当前选中的接口:");
			selectedPort.name = GUILayout.TextField(selectedPort.name);
			GUILayout.EndHorizontal();

			// 显示端口位置
			EditorGUI.BeginChangeCheck();
			selectedPort.Position = EditorGUILayout.Vector3Field("位置：", selectedPort.Position);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(selectedPort, "Change position");
				EditorUtility.SetDirty(selectedPort);
				var newRot = Quaternion.Euler(new Vector3(0, 0, selectedPort.Position.z));
				var newPos = selectedPort.Position;
				newPos.z = 0;
				selectedPort.transform.localPosition = newPos;
				selectedPort.transform.rotation = newRot;
			}

			// 显示端口类型
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(serializedPort.FindProperty("suitableType"));
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(selectedPort, "Change suitable type");
				EditorUtility.SetDirty(selectedPort);
				serializedPort.ApplyModifiedProperties();
			}

		}

		/// <summary>
		/// 绘制端口方向箭头
		/// </summary>
		/// <param name="position"></param>
		/// <param name="rotation"></param>
		/// <param name="length"></param>
		void DrawDirectionArrow(Vector3 position, Quaternion rotation, float length)
		{
			var dir = rotation * Vector3.up * length;
			var lines = new Vector3[5];
			lines[0] = position;
			lines[1] = dir + lines[0];
			lines[2] = Quaternion.Euler(0, 0, 160) * dir * 0.5f + lines[1];
			lines[3] = Quaternion.Euler(0, 0, -160) * dir * 0.5f + lines[1];
			lines[4] = lines[1];
			Handles.DrawPolyLine(lines);
		}
	}
}
