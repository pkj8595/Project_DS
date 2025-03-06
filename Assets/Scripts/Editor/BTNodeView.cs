using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

public class BTNodeView : Node
{
    public BTNode node;

    public Port inputPort;
    public Port outputPort;

    public BTNodeView(BTNode node)
    {
        this.node = node;
        title = node.name;

        // 입력 포트 생성
        inputPort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        inputPort.portName = "Input";
        inputContainer.Add(inputPort);

        // 출력 포트 생성 (자식 연결)
        outputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
        outputPort.portName = "Output";
        outputContainer.Add(outputPort);

        RefreshExpandedState();
        RefreshPorts();
    }
}
