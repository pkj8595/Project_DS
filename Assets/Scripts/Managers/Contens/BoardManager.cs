using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.AI;
using Unity.AI.Navigation;

public class BoardManager : MonoSingleton<BoardManager>
{
    enum EBuildingStep
    {
        None,
        PositionStep,
        RotationStep,
    }

    private MeshFilter mergedMeshFilter;
    private MeshRenderer mergedMeshRenderer;
    private MeshCollider mergedMeshCollider;
    [SerializeField] private GameObject _combineMeshObject;
    [SerializeField] private GameObject _nodeGroup;
    [SerializeField] private GameObject _buildingGroup;
    //[SerializeField] private NavMeshSurface navSurface;
    [SerializeField] private NavMeshSurface navSurface;

    [SerializeField] private List<GameObject> _nodeList;
    [SerializeField] private List<GameObject> _buildingList;

    [SerializeField] private Dictionary<Vector3Int, NodeBase> _dirNodes = new();
    private List<BuildingNode> _constructedBuildingList { get => GameView.Instance.ConstructedBuildingList; }
    [SerializeField] private Vector3 tileSize = new Vector3(1, 1, 1); // 각 셀의 크기

    private bool _isEditMode = false;
    private bool _isSelectNode = false;
    private int _selectedNodeIndex = -1;
    private EBuildingStep _cardBuildingStep = EBuildingStep.None;

    [SerializeField] private LineRenderer _lineRender;

    private NodeBase _previewNode;
    [SerializeField] private Material _previewMaterial_Green;
    [SerializeField] private Material _previewMaterial_Red;

    private (Vector3Int position, Vector3 normal) _beforeMousePosition;
    private string _cardItemPath;

    private void Start()
    {
        if (_combineMeshObject == null)
        {
            _combineMeshObject = GameObject.Find("CombineMesh");
        }

        if (navSurface == null)
        {
            navSurface = GetComponentInChildren<NavMeshSurface>();
        }

        mergedMeshFilter = _combineMeshObject.GetOrAddComponent<MeshFilter>();
        mergedMeshRenderer = _combineMeshObject.GetOrAddComponent<MeshRenderer>();
        mergedMeshCollider = _combineMeshObject.GetOrAddComponent<MeshCollider>();

        LoadBoard();
    }

    void Update()
    {
        if (_isEditMode && _isSelectNode || _cardBuildingStep == EBuildingStep.PositionStep)
        {
            //프리뷰 노드 보여주기
            (Vector3Int position, Vector3 normal) nodeMouse = GetCellPositionToMouse();
            if (Input.GetKeyDown(KeyCode.F))
                Debug.Log(nodeMouse);
            if (_beforeMousePosition.position == nodeMouse.position &&
                _beforeMousePosition.normal == nodeMouse.normal)
                return;
            _beforeMousePosition = nodeMouse;

            //스케일 비례 포지션 
            _previewNode.transform.position = ComputeNodeScalePosition(_previewNode, nodeMouse.position);
            //node 포지션
            _previewNode.Position = nodeMouse.position;
            //_previewNode.SetNodeRotation(nodeMouse.normal);

            ChangeMaterialPreviewNode(CanPlaceBuilding(nodeMouse.position, _previewNode.NodeSize, _previewNode));
        }
        else if(_cardBuildingStep == EBuildingStep.RotationStep)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                Vector3 worldMousePosition = hit.point;
                Vector3 direction = worldMousePosition - _previewNode.transform.position;
                direction.y = 0;

                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    float snappedYRotation = Mathf.Round(targetRotation.eulerAngles.y / 90f) * 90f;
                    _previewNode.transform.rotation = Quaternion.Euler(0, snappedYRotation, 0);
                }

                Vector3[] arrLine = { _previewNode.transform.position, 
                                    new Vector3(worldMousePosition.x,
                                                _previewNode.transform.position.y,
                                                worldMousePosition.z) };
                _lineRender.SetPositions(arrLine);
            }

            if (CanPlaceBuilding(_previewNode.Position, _previewNode.NodeSize, _previewNode))
            {
                ChangeMaterialPreviewNode(true);
                //회전 상태에서 
                if (Input.GetMouseButtonDown(0))
                {
                    _card.UseComplete(CompleteCardBuilding());
                }
            }
            else
            {
                ChangeMaterialPreviewNode(false);
            }


            //오른쪽 마우스 클릭 -> 캔슬
            if (Input.GetMouseButtonDown(1))
            {
                _card.UseComplete(false);
                OnCancelCard();
            }
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(0))
        {
            Debug.Log($"{GetCellPositionToMouse()}");
        }
    }

    private void OnKeyAction()
    {
        if (Input.GetKeyDown(KeyCode.R) && _isEditMode )
            RemoveTile();

        if (Input.GetKeyDown(KeyCode.Space))
            ChangeEditMode();
    }

    private void OnMouseAction(Define.MouseEvent evt)
    {
        if (Define.MouseEvent.LPointerDown == evt && _isEditMode && _isSelectNode)
        {
            CreateNode_List(_selectedNodeIndex);
        }
    }

    private void ChangeEditMode()
    {
        _isEditMode = !_isEditMode;
        
        if (_isEditMode)
        {
            UnmergeMeshes();
            var uiBoard = Managers.UI.ShowUI<UIBoard>();
            uiBoard.SetActive(_isEditMode);
           
        }
        else
        {
            _isSelectNode = false;
            _selectedNodeIndex = -1;
            ClearPreviewNode();
            MergeAllMeshes();
            Managers.UI.CloseUI<UIBoard>();
        }
    }

    #region save & load
    //todo : saveload
    private void SaveBoard()
    {

    }

    private void LoadBoard()
    {
        _dirNodes.Clear();
        var nodes = _nodeGroup.GetComponentsInChildren<NodeBase>();
        for (int i = 0; i < nodes.Length; i++)
        {
            SetNodeInDic(nodes[i]);
        }

        var buildingNodes = _buildingGroup.GetComponentsInChildren<NodeBase>();
        for (int i = 0; i < buildingNodes.Length; i++)
        {
            SetNodeInDic(buildingNodes[i]);
        }

        MergeAllMeshes();
    }
    #endregion

    private void SetNodeInDic(NodeBase node)
    {
        if (node is BuildingNode)
        {
            _constructedBuildingList.Add(node as BuildingNode);
        }

        //todo
        int xOffset = Mathf.RoundToInt((node.NodeSize.x - 1) * 0.5f);
        int zOffset = Mathf.RoundToInt((node.NodeSize.z - 1) * 0.5f);
        int startX = Mathf.RoundToInt(node.transform.position.x) - xOffset;
        int startZ = Mathf.RoundToInt(node.transform.position.z) - zOffset;
        int startY = Mathf.RoundToInt(node.transform.position.y);

        for (int y = 0; y < node.NodeSize.y; y++)
        {
            for (int x = 0; x < node.NodeSize.x; x++)
            {
                for (int z = 0; z < node.NodeSize.z; z++)
                {
                    Vector3Int nodePosition = new Vector3Int(startX + x,
                                                             startY + y,
                                                             startZ + z);
                    _dirNodes.Add(nodePosition, node);
                    
                }
            }
        }
        node.Init(new Vector3Int(startX, startY, startZ));
        node.InstallationSuccess();

    }

    [ContextMenu("ShowNodeDic")]
    public void ShowNodeDic()
    {
        System.Text.StringBuilder stringBuilder = new();
        foreach(var item in _dirNodes)
        {
            stringBuilder.Append($"{item.Key} {item.Value.name}\n" );
        }
        Debug.Log(stringBuilder.ToString());
    }


    #region create & remove
    private void CreateNode_List(int index)
    {
        if (!(0 <= index && index < _nodeList.Count))
        {
            Debug.Log($"index가 유효하지 않습니다.{index}");
            return;
        }

        //AddBuildingNode(_previewNode, _nodeList[index]);
    }


    private void RemoveTile()
    {
        NodeBase node = GetNodeToMouse();
        if (node == null)
            return;

        RemoveNode(node);
    }

    #endregion

    private NodeBase GetNodeToMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int layerMask = (int)Define.Layer.Ground | (int)Define.Layer.Building;
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 100f, layerMask))
        {
            if(raycastHit.collider != null)
                return raycastHit.collider.GetComponent<NodeBase>();
            if (raycastHit.rigidbody != null)
                return raycastHit.rigidbody.GetComponent<NodeBase>();
            Debug.Log("collider,rigidbody 검출 실패");
        }
        return null;
    }

    //현재 마우스 위치를 참조해 Grid의 좌표를 가져온다.
    private (Vector3Int, Vector3) GetCellPositionToMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int layerMask = (int)Define.Layer.Water | (int)Define.Layer.Ground;

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, layerMask))
        {
            if (1 << hit.transform.gameObject.layer == (int)Define.Layer.Water)
            {
                return (WorldToGrid(hit.point, hit.normal), Vector3.zero);
            }
            else /*if (1 << hit.transform.gameObject.layer == (int)Define.Layer.Ground)*/
            {
                return (WorldToGrid(hit.point, hit.normal), hit.normal);
            }
        }
        return (Vector3Int.zero, Vector3.zero);
    }

    private bool AddBuildingNode(NodeBase previewNode)
    {
       
        return AddBuildingNode(previewNode, true);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="previewNode"></param>
    /// <param name="nodePrefab"></param>
    private bool AddBuildingNode(NodeBase previewNode, bool isCard = false)
    {
        NodeBase node = previewNode.GetComponent<NodeBase>();
        if (node == null)
        {
            Debug.Log($"{previewNode.name}이 없습니다.");
            return false;
        }

        Vector3Int gridPosition = previewNode.Position;
        if (!CanPlaceBuilding(gridPosition, node.NodeSize, previewNode))
        {
            Debug.Log("이미 설치된 위치입니다.");
            return false;
        }

        //생성 셋팅
        bool isBuildingNode = previewNode is BuildingNode;
        GameObject nodeprefab = Managers.Resource.Instantiate(_cardItemPath, isBuildingNode ? _buildingGroup.transform : this.transform);
        NodeBase nodeObject = nodeprefab.GetComponent<NodeBase>();
        // node 생성 및 초기화
        nodeObject.gameObject.SetActive(false);
        nodeObject.Init(gridPosition);
        nodeObject.transform.position = previewNode.transform.position;
        nodeObject.transform.rotation = previewNode.transform.rotation;

        if (isBuildingNode)
        {
            _constructedBuildingList.Add(nodeObject as BuildingNode);
        }
        else
        {
            //blockNode 일 경우 메쉬 병합
            nodeObject.SetActiveCompleteAction(() => {
                AddMesh(nodeObject.GetComponentsInChildren<MeshFilter>());
                nodeObject.transform.SetParent(_nodeGroup.transform);
            });
        }

        // 차지하는 공간을 nodes 딕셔너리에 추가
        for (int y = 0; y < node.NodeSize.y; y++)
        {
            for (int x = 0; x < node.NodeSize.x; x++)
            {
                for (int z = 0; z < node.NodeSize.z; z++)
                {
                    Vector3Int nodePosition = new Vector3Int(gridPosition.x + x,
                                                             gridPosition.y + y,
                                                             gridPosition.z + z);
                    _dirNodes.Add(nodePosition, nodeObject);
                }
            }
        }

        nodeObject.SetActive(true);
        //설치 성공시 실행
        nodeObject.InstallationSuccess();
        return true;
    }

    private static Vector3 ComputeNodeScalePosition(NodeBase node, Vector3Int gridPosition)
    {
        return gridPosition + new Vector3((node.NodeSize.x - 1) * 0.5f, 0, (node.NodeSize.z - 1) * 0.5f);
    }

    // 건물 노드 제거
    public void RemoveNode(NodeBase node)
    {
        // building 일 경우 리스트에서 삭제
        if( node is BuildingNode)
        {
            _constructedBuildingList.Remove(node as BuildingNode);
        }

        // 차지하는 공간을 nodes 딕셔너리에서 제거
        for (int y = 0; y < node.NodeSize.y; y++)
        {
            for (int x = 0; x < node.NodeSize.x; x++)
            {
                for (int z = 0; z < node.NodeSize.z; z++)
                {
                    Vector3Int nodePosition = new Vector3Int(node.Position.x + x,
                                                             node.Position.y + y,
                                                             node.Position.z + z);
                    _dirNodes.Remove(nodePosition);
                }
            }
        }
        Managers.Resource.Destroy(node.gameObject);
    }


    private bool CanPlaceBuilding(Vector3Int gridPosition, Vector3Int size, NodeBase node)
    {

        //블록노드의 경우 y가 0이 아니라면 아래 blocknode를 찾는다.
        if(node is BlockNode)
        {
            if (gridPosition.y != 0)
            {
                for (int x = 0; x < size.x; x++)
                {
                    for (int z = 0; z < size.z; z++)
                    {
                        Vector3Int nodePosition = new Vector3Int(gridPosition.x + x, gridPosition.y - 1, gridPosition.z + z);
                        if (_dirNodes.ContainsKey(nodePosition))
                        {
                            if (_dirNodes[nodePosition] is not BlockNode)
                                return false;
                        }
                        else
                            return false;
                    }
                }
            }
        }
        else
        {
            for (int x = 0; x < size.x; x++)
            {
                for (int z = 0; z < size.z; z++)
                {
                    Vector3Int nodePosition = new Vector3Int(gridPosition.x + x, gridPosition.y - 1, gridPosition.z + z);
                    if (_dirNodes.ContainsKey(nodePosition))
                    {
                        if (_dirNodes[nodePosition] is not BlockNode)
                            return false;
                    }
                    else
                        return false;
                }
            }
        }

        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                for (int z = 0; z < size.z; z++)
                {
                    Vector3Int nodePosition = new Vector3Int(gridPosition.x + x, gridPosition.y + y, gridPosition.z + z);
                    if (_dirNodes.ContainsKey(nodePosition))
                    {
                        //Debug.Log($"field : x{x},y{y},z{z} nodePosition : {nodePosition} ");
                        return false; // 이미 노드가 있는 위치에는 건물을 지을 수 없음
                    }
                }
            }
        }
        return true; // 모든 위치가 비어있으면 건물을 지을 수 있음
    }

    // 회전된 좌표 계산 (90도 단위로 처리) + 회전 오프셋 적용
    private Vector3Int GetRotatedPosition(Vector3Int origin, int x, int y, int z, int rotation)
    {
        rotation = (rotation + 360) % 360;
        switch (rotation)
        {
            case 90:
                return new Vector3Int(origin.x - z, origin.y + y, origin.z + x);
            case 180:
                return new Vector3Int(origin.x - x, origin.y + y, origin.z - z);
            case 270:
                return new Vector3Int(origin.x + z, origin.y + y, origin.z - x);
            default:
                return new Vector3Int(origin.x + x, origin.y + y, origin.z + z); // 0도 (front)
        }
    }

    /// <summary>
    /// 반올림 
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public Vector3Int WorldToGrid(Vector3 point, Vector3 normal)
    {
        if (normal == Vector3.zero || normal.x + normal.y + normal.z > 0)
            return Vector3Int.FloorToInt(point + (Vector3.one * 0.5f));

        return Vector3Int.FloorToInt(point + (Vector3.one * 0.5f)) + Vector3Int.FloorToInt(normal);
    }

    /// <summary>
    /// 인수로 받은 worldPosition에서 Navmesh로 이동 가능한 가장 가까운 Position 반환
    /// </summary>
    /// <param name="worldPosition">world position</param>
    /// <param name="movealbePosition">이동 가능한 world position 반환</param>
    /// <returns>NavMesh.SamplePosition 성공 여부</returns>
    public bool GetMoveablePosition(Vector3 worldPosition, out Vector3 movealbePosition, float maxDistance = 1f)
    {
        movealbePosition = Vector3.zero;
        if (UnityEngine.AI.NavMesh.SamplePosition(worldPosition, out UnityEngine.AI.NavMeshHit hit, maxDistance, UnityEngine.AI.NavMesh.AllAreas))
        {
            movealbePosition = hit.position;
            return true;
        }

        return false;
    }

    #region mesh
    /// <summary>
    /// Node 메쉬 병합
    /// </summary>
    private void MergeAllMeshes()
    {
        //_nodeGroup 하위 nodeBlock의 메쉬가져오기 
        MeshFilter[] meshFilters = _nodeGroup.GetComponentsInChildren<MeshFilter>();

        if (meshFilters.Length == 0) return;

        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combine, true);

        mergedMeshFilter.mesh = combinedMesh;
        mergedMeshCollider.sharedMesh = combinedMesh;

        // 병합된 메쉬의 재질 설정 (첫 번째 메쉬의 재질 사용)
        if (meshFilters.Length > 0)
        {
            mergedMeshRenderer.sharedMaterial = meshFilters[0].GetComponent<MeshRenderer>().sharedMaterial;
        }

        // 병합 후 node group 비활성화
        _nodeGroup.gameObject.SetActive(false);
        navSurface.BuildNavMesh();
    }

    private void AddMesh(MeshFilter[] meshFilters)
    {
        CombineInstance[] combine = new CombineInstance[meshFilters.Length + 1];
        
        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
        }
        combine[meshFilters.Length].mesh = mergedMeshFilter.sharedMesh;
        combine[meshFilters.Length].transform = mergedMeshFilter.transform.localToWorldMatrix;

        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combine, true);

        mergedMeshFilter.mesh = combinedMesh;
        mergedMeshCollider.sharedMesh = combinedMesh;
        mergedMeshRenderer.sharedMaterial = mergedMeshFilter.GetComponent<MeshRenderer>().sharedMaterial;

        _nodeGroup.gameObject.SetActive(false);
        navSurface.BuildNavMesh();
    }

    /// <summary>
    /// 병합 해제
    /// </summary>
    private void UnmergeMeshes()
    {
        if (mergedMeshFilter.mesh != null)
        {
            Destroy(mergedMeshFilter.mesh);
            mergedMeshFilter.mesh = null;
            mergedMeshCollider.sharedMesh = null;
        }

        // nodeGroup 활성화
        _nodeGroup.gameObject.SetActive(true);
    }
    #endregion

    /// <summary>
    /// 보드 UI에서 생성 
    /// </summary>
    /// <param name="index"></param>
    public void SetNodeIndex(int index)
    {
        if (!_isEditMode)
            return;
        if (_selectedNodeIndex == index)
            return;

        ClearPreviewNode();
        _selectedNodeIndex = index;
        _previewNode = Managers.Resource.Instantiate(_nodeList[_selectedNodeIndex], this.transform)
                                        .GetComponent<NodeBase>();
        _previewNode.gameObject.layer = 2;
        _previewNode.gameObject.name = "PreviewNode";
        ChangeMaterialPreviewNode(false);
        var navObstacle = _previewNode.GetComponent<UnityEngine.AI.NavMeshObstacle>();
        navObstacle.enabled = false;
        _previewNode.gameObject.SetActive(true);

        _isSelectNode = true;

    }
    
    /// <summary>
    /// 카드에서 생성
    /// </summary>
    /// <param name="itemBase"></param>
    public void ExcuteCardBuilding(ItemBase itemBase)
    {
        ClearPreviewNode();
        if (itemBase is BuildingItem)
            _cardItemPath = $"Building/{Managers.Data.BuildingDict[itemBase.GetTableNum].prefab}";
        else if(itemBase is TileItem)
            _cardItemPath = $"Node/{Managers.Data.TileBaseDict[itemBase.GetTableNum].prefab}";

        _previewNode = Managers.Resource.Instantiate(_cardItemPath, this.transform)
                                        .GetComponent<NodeBase>();
        _previewNode.gameObject.layer = 0;
        _previewNode.gameObject.name = "PreviewNode";
        ChangeMaterialPreviewNode(false);
        _previewNode.SetActive(true);
        _cardBuildingStep = EBuildingStep.PositionStep;
    }

    UICard _card;
    public bool RotationStep(UICard card)
    {
        _card = card;
        
        if (_previewNode != null && CanPlaceBuilding(_beforeMousePosition.position, _previewNode.NodeSize, _previewNode))
        {
            _cardBuildingStep = EBuildingStep.RotationStep;
            _lineRender.enabled = true;
            return true;
        }
        OnCancelCard();

        return false;
    }

    /// <summary>
    /// 카드를 놓았을때 실행
    /// </summary>
    /// <param name="itemBase"></param>
    public bool CompleteCardBuilding()
    {
        bool isBuilding = AddBuildingNode(_previewNode);
        ClearPreviewNode();
        ClearCard();
        return isBuilding;
    }

    private void ClearCard()
    {
        _card = null;
        _cardBuildingStep = EBuildingStep.None;
        _cardItemPath = string.Empty;
        _lineRender.enabled = false;
    }

    public void OnCancelCard()
    {
        if(_card != null)
            _card.UseComplete(false);
        ClearPreviewNode();
        ClearCard();
    }

    public void OnCancelSelectedNode()
    {
        ClearPreviewNode();
        _isSelectNode = false;
    }

    private void ChangeMaterialPreviewNode(bool isCan)
    {
        MeshRenderer[] meshRender = _previewNode.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < meshRender.Length; i++)
        {
            meshRender[i].material = isCan ? _previewMaterial_Green : _previewMaterial_Red;
        }
    }

    private void ClearPreviewNode()
    {
        if (_previewNode != null)
           Destroy(_previewNode.gameObject);
        _previewNode = null;
    }


}
