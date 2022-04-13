using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System.IO;


public class Game_Handler : MonoBehaviourPunCallbacks
{
    public static string mapFileName;
    [SerializeField] PhotonView LocalView;
    [SerializeField] SaveLoad_Handler_Script saveLoad;
    [SerializeField] public World_Handler_Script worldHandler;

    List<World_Handler_Script.WorldTileSpawnPoints> spawnZones;

    public static CharacterData[] PlayerUnits;
    public enum Team
    {
        Team1,
        Team2,
        Team3,
        Team4,
        Team5,
        Team6,
        Team7,
        fail
    };
    Team localTeam;
    Team currentActiveTeam;
    int currentActiveTeamIndex = 0;
    Photon.Realtime.Player[] OtherPlayers;
    Photon.Realtime.Player[] TotalPlayers;
    Dictionary<Team, Photon.Realtime.Player> TeamToPlayerDictionary = new Dictionary<Team, Photon.Realtime.Player>();
    int teamIntValue;
    [SerializeField] Button nextTurnButton;
    //game setup
    void Start()
    {
        pathfindingVisualHandler = GameObject.FindGameObjectWithTag("PathfindingVisual").GetComponent<pathfindingColorVisualHandler>();
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        setMap(mapFileName);
        LocalView.RPC("setMap", RpcTarget.OthersBuffered, mapFileName);
        saveLoad.load(mapFileName);
        OtherPlayers = PhotonNetwork.PlayerListOthers;
        TotalPlayers = PhotonNetwork.PlayerList;
        AssignTeams();
        SetActiveTeam(Team.Team1);

        for (int i = 0; i < TotalPlayers.Length; i++)
        {
            TeamToPlayerDictionary.TryGetValue(getTeamFromValue(i), out Photon.Realtime.Player plr);
            //Debug.Log("RPC SpawnUnits: " + i);
            LocalView.RPC("spawnUnits", plr, i);
        }
    }



    [PunRPC]
    public void assignTeamInt(int i)
    {
        teamIntValue = i;
    }

    void AssignTeams()
    {
        for (int i = 0; i < TotalPlayers.Length; i++)
        {
            LocalView.RPC("AssignLocalTeam", TotalPlayers[i], getTeamFromValue(i));
            TeamToPlayerDictionary.Add(getTeamFromValue(i), TotalPlayers[i]);
            LocalView.RPC("assignTeamInt", TotalPlayers[i], getIntFromTeam(getTeamFromValue(i)));
        }
    }

    Team getTeamFromValue(int index)
    {
        switch (index)
        {
            case 0: return Team.Team1;
            case 1: return Team.Team2;
            case 2: return Team.Team3;
            case 3: return Team.Team4;
            case 4: return Team.Team5;
            case 5: return Team.Team6;
            case 6: return Team.Team7;
        }
        return Team.fail;
    }

    int getIntFromTeam(Team team)
    {
        switch (team)
        {
            case Team.Team1:
                return 0;
            case Team.Team2:
                return 1;
            case Team.Team3:
                return 2;
            case Team.Team4:
                return 3;
            case Team.Team5:
                return 4;
            case Team.Team6:
                return 5;
            case Team.Team7:
                return 6;
        }
        return -1;
    }


    [PunRPC]
    void AssignLocalTeam(Team team)
    {
        localTeam = team;
    }

    [PunRPC]
    void SetActiveTeam(Team activeTeam)
    {
        currentActiveTeam = activeTeam;
        Debug.Log("Active Team:" + activeTeam);
    }

    [PunRPC]
    public void setMap(string mapFileName)
    {
        Game_Handler.mapFileName = mapFileName.Trim();
        saveLoad.load(mapFileName);
        spawnZones = worldHandler.setupSpawnPoints(worldHandler.getBuildLayers());
    }

    [PunRPC]
    void spawnUnits(int spawnZoneIndex)
    {
        //Debug.Log("PlayerUnits Length:" + PlayerUnits.Length + ", SpawnZoneCount:" + spawnZones.Count);
        World_Handler_Script.WorldTileSpawnPoints spawnZone = spawnZones[spawnZoneIndex];
        Camera.main.transform.position = worldHandler.getBuildLayers().getWorldPosition(spawnZone.TilesPos[0].x, spawnZone.TilesPos[0].y);
        Vector2Int[] arr = new Vector2Int[PlayerUnits.Length];
        for (int i = 0; i < PlayerUnits.Length; i++)
        {
            spawnUnit(spawnZone.TilesPos[i % spawnZone.TilesPos.Count], i);
            arr[i] = spawnZone.TilesPos[i % spawnZone.TilesPos.Count];
        }

        if (!PhotonNetwork.IsMasterClient)
        {
            LocalView.RPC("GiveMasterNewPositions", Photon.Pun.RpcTarget.MasterClient, Vector2IntArrayToString(arr), PhotonNetwork.LocalPlayer);
        }
        else
        {
            GiveMasterNewPositions(Vector2IntArrayToString(arr), PhotonNetwork.LocalPlayer);
        }
    }


    [SerializeField] GameObject UnitPrefab;
    public void spawnUnit(Vector2Int tile, int index)
    {
        //Debug.Log("SpawnedUnit At:" + tile.ToString());
        Vector3 worldPosToSpawnAt = worldHandler.getBuildLayers().getWorldPosition(tile.x, tile.y) + (new Vector3(1, 1) * worldHandler.cellSize) * .5f;
        GameObject unit = PhotonNetwork.Instantiate("UnitPreFab", worldPosToSpawnAt, Quaternion.identity);  //Instantiate(UnitPrefab);

        InGame_Unit_Handler_Script unitHandler = unit.GetComponent<InGame_Unit_Handler_Script>();
        unitHandler.setup(worldPosToSpawnAt, PlayerUnits[index], tile);
        unitHandler.hideOtherSelf();
        AllUnits.Add(unitHandler);
    }

    //turn handleing


    [PunRPC]
    void MasterNextTurn()
    {
        currentActiveTeamIndex = (currentActiveTeamIndex + 1) % TotalPlayers.Length;
        LocalView.RPC("recieveNextTurn", RpcTarget.All, currentActiveTeamIndex);
    }

    [PunRPC]
    void recieveNextTurn(int index)
    {
        currentActiveTeamIndex = index;
        currentActiveTeam = getTeamFromValue(currentActiveTeamIndex);
        Debug.Log("Next Turn Set To:" + currentActiveTeam);
    }

    public void EndMyTurn()
    {
        CancelMoveRequest = true;
        if (IsMyTurn())
        {
            selectFriendlyUnit(null);
            LocalView.RPC("MasterNextTurn", RpcTarget.MasterClient);
        }
    }




    //gameStateHandling


    public bool IsMyTurn()
    {
        if (currentActiveTeam == localTeam)
        {
            return true;
        }
        return false;
    }




    public InGame_Unit_Handler_Script SelectedUnit;

    void selectFriendlyUnit(InGame_Unit_Handler_Script selected)
    {
        if (selected != SelectedUnit)
        {
            resetShooting();
        }
        else
        {
            return;
        }
        if (SelectedUnit != null)
        {
            SelectedUnit.isSelected = false;
        }
        SelectedUnit = selected;
        if (SelectedUnit != null)
        {
            SelectedUnit.isSelected = true;
        }
    }

    //local units
    public List<InGame_Unit_Handler_Script> AllUnits = new List<InGame_Unit_Handler_Script>();

    [SerializeField] bool AllowDebuggingInputs = false;
    bool firstUpdateOnTurn = true;
    // Update is called once per frame
    void Update()
    {
        if (IsMyTurn())
        {
            nextTurnButton.interactable = true;
            if (firstUpdateOnTurn)
            {
                //first frame of turn
                firstUpdateOnTurn = false;
                foreach (InGame_Unit_Handler_Script scr in AllUnits)
                {
                    scr.resetValuesOnStartOfTurn();
                }
            }

            if (Input.GetMouseButtonDown(0) && !UtilClass.IsPointerOverUIElement(LayerMask.NameToLayer("UI")))
            {
                handleLeftClick();
            }
            else if (Input.GetMouseButtonDown(1) && !UtilClass.IsPointerOverUIElement(LayerMask.NameToLayer("UI")))
            {
                handleRightClick();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1) && AllowDebuggingInputs)
            {
                //Debug.Log("stop moving");
                stopAllUnitMovement();
            }
            //Debug.Log("AllUnitsInGameCount:" + allUnitsInGame.Count);
        }
        else
        {
            firstUpdateOnTurn = true;
            handleUnitDeath();
            nextTurnButton.interactable = false;
        }
    }

    void handleUnitDeath()
    {
        List<InGame_Unit_Handler_Script> toRemove = new List<InGame_Unit_Handler_Script>();
        foreach (InGame_Unit_Handler_Script scr in AllUnits)
        {
            if (scr.currentHealth <= 0)
            {
                scr.handleDeath();
                toRemove.Add(scr);
            }
        }
        foreach (InGame_Unit_Handler_Script scr in toRemove)
        {
            AllUnits.Remove(scr);
        }
        if (toRemove.Count > 0)
        {
            List<Vector2Int> poses = new List<Vector2Int>();
            foreach (InGame_Unit_Handler_Script scr in AllUnits)
            {
                poses.Add(scr.gridPos);
            }
            LocalView.RPC("GiveMasterNewPositions", RpcTarget.MasterClient, Vector2IntArrayToString(poses.ToArray()), PhotonNetwork.LocalPlayer);
        }
        if (AllUnits.Count == 0)
        {
            if (!loseConditionHandled)
            {
                loseConditionHandled = true;
                handleAllFriendlyUnitsDead();
            }
        }

    }


    void handleRightClick()
    {
        handleUnitRightClick();
    }

    void handleLeftClick()
    {
        handleUnitLeftClick();
    }


    void handleUnitRightClick()
    {
        if (WaitingForAcceptionOfPath)
        {
            Debug.Log("cancelMoveRequest");
            CancelMoveRequest = true;
            resetPathVisualGrid();
        }
        else
        {
            selectFriendlyUnit(null);
        }
    }

    [SerializeField] bool adjacentOnly = false;
    void handleUnitLeftClick()
    {
        if (IsMouseOverUI())
        {
            handleUILeftClick();
        }
        else
        {
            InGame_Unit_Handler_Script tempSelect = null;
            foreach (InGame_Unit_Handler_Script scr in AllUnits)
            {
                if (scr.mouseOver)
                {
                    tempSelect = scr;
                }
            }

            if (tempSelect != null)
            {
                //unit clicked
                selectFriendlyUnit(tempSelect);
                resetPathVisualGrid();
            }
            else
            {
                worldHandler.getBuildLayers().GetXY(UtilClass.getMouseWorldPosition(), out int x, out int y);
                //no unit clicked
                if (getRestrictedTiles().Contains(new Vector2Int(x, y)) && SelectedUnit != null)
                {
                    //clicking on enemy Unit
                    HandleClickOnEnemyUnit();
                }
                else if (SelectedUnit != null && !moving && !getRestrictedTiles().Contains(new Vector2Int(x, y)))
                {
                    //trying to path

                    resetShooting();
                    List<Vector2Int> restricted = getRestrictedTiles();
                    bool empty = true;
                    if (worldHandler.getBuildLayers().inBounds(x, y))
                    {
                        //invalid pos

                        foreach (Vector2Int vec in restricted)
                        {
                            if (vec == new Vector2Int(x, y))
                            {
                                empty = false;
                            }
                        }

                        if (empty)
                        {
                            //clicked on empty tile
                            handleMove();
                        }
                        else
                        {
                            //clicked on occupied Tile
                            Debug.Log("Clicked On Occupied Tile");
                        }
                    }
                }

            }
        }
    }




    void stopAllUnitMovement()
    {
        foreach (InGame_Unit_Handler_Script unit in AllUnits)
        {
            if (unit.needToMove)
            {
                StartCoroutine(unit.stopMovement());
            }
        }
    }





    private void OnApplicationQuit()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            List<Vector2Int> poses = new List<Vector2Int>();
            foreach (InGame_Unit_Handler_Script scr in AllUnits)
            {
                poses.Add(scr.gridPos);
            }
            LocalView.RPC("GiveMasterNewPositions", RpcTarget.MasterClient, Vector2IntArrayToString(poses.ToArray()), PhotonNetwork.LocalPlayer);
        }
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        masterClientLeft();
    }



    private void masterClientLeft()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.AutomaticallySyncScene = false;
        SceneManager.LoadScene("JoinServer");
    }

    public void leave()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            List<Vector2Int> poses = new List<Vector2Int>();
            foreach (InGame_Unit_Handler_Script scr in AllUnits)
            {
                poses.Add(scr.gridPos);
            }
            LocalView.RPC("GiveMasterNewPositions", RpcTarget.MasterClient, Vector2IntArrayToString(poses.ToArray()), PhotonNetwork.LocalPlayer);
        }
        masterClientLeft();
    }










    void handleMove()
    {
        if (!WaitingForAcceptionOfPath)
        {
            //new path
            StartCoroutine(handleUnitMove());
            WaitingForAcceptionOfPath = true;
        }
        else
        {
            //accept path
            AcceptedMovePath = true;
        }
    }



    private void resetPathVisualGrid()
    {

        for (int i = 0; i < worldHandler.getBuildLayers().width; i++)
        {
            for (int j = 0; j < worldHandler.getBuildLayers().height; j++)
            {
                worldHandler.getBuildLayers().getGridObject(i, j).setPathfindingData(null);
            }
        }
        //Debug.Log("kill old path");
        pathfindingVisualHandler.SetGrid(worldHandler.getBuildLayers());
    }




    bool moving = false;
    bool AcceptedMovePath = false;
    bool CancelMoveRequest = false;
    bool WaitingForAcceptionOfPath = false;

    [SerializeField] int greenColorDist = 80;
    [SerializeField] int orangeColorDist = 160;
    [SerializeField] int RedColorDist = 240;
    int tempActionPointsMax;
    int tempActionPointsCur;
    private int calculateUnitMoveColors(World_Handler_Script.WorldBuildTile tile)
    {
        tempActionPointsCur = tempActionPointsMax;
        tempActionPointsCur -= (int)(tile.gCost * SelectedUnit.getMoveCostMultiplier());
        if ((float)tempActionPointsCur / (float)tempActionPointsMax > .5)
        {
            return 2;
        }
        else if (tempActionPointsCur > 0)
        {
            return 1;
        }
        else
        {
            return 0;
        }

    }

    private IEnumerator handleUnitMove()
    {
        //Debug.Log("start new path");

        //reset path visuals
        resetPathVisualGrid();



        //unit selected and empty grid square clicked
        //do pathfinding

        worldHandler.getBuildLayers().GetXY(SelectedUnit.transform.position, out int x, out int y);
        worldHandler.getBuildLayers().GetXY(UtilClass.getMouseWorldPosition(), out int x2, out int y2);
        if (!worldHandler.getBuildLayers().inBounds(x, y) || !worldHandler.getBuildLayers().inBounds(x2, y2))
        {
            //invalid pos
            yield break;
        }

        if (clickedPosOccupied(new Vector2Int(x2, y2)))
        {
            //Debug.Log("spot already used");
            yield break;
        }


        Vector2Int[] path = new Vector2Int[0];
        yield return StartCoroutine(AstarPathing.returnPath(new Vector2Int(x, y), new Vector2Int(x2, y2), worldHandler.getBuildLayers(), adjacentOnly, getRestrictedTiles(), (pathReturn) =>
        {
            //Debug.Log("The World Is Ending");
            path = pathReturn;
        }));
        if (path != null && path.Length > 0)
        {
            tempActionPointsMax = SelectedUnit.currentActionPoints;
            tempActionPointsCur = SelectedUnit.currentActionPoints;
            foreach (Vector2Int vec in path)
            {
                worldHandler.getBuildLayers().getGridObject(vec.x, vec.y).setPathfindingData(pathfindingColors[calculateUnitMoveColors(worldHandler.getBuildLayers().getGridObject(vec.x, vec.y))]);
            }
            pathfindingVisualHandler.SetGrid(worldHandler.getBuildLayers());
        }


        //Debug.Log("The World Is Starting");
        if (path == null)
        {
            Debug.Log("invalid path null");
        }
        else if (path.Length == 0)
        {

            Debug.Log("invalid path no length");
        }
        else
        {

            //wait for acceptance of path
            while (!AcceptedMovePath)
            {
                if (CancelMoveRequest)
                {
                    AcceptedMovePath = false;
                    CancelMoveRequest = false;
                    WaitingForAcceptionOfPath = false;

                    resetPathVisualGrid();

                    yield break;
                }
                yield return null;
            }
            worldHandler.getBuildLayers().GetXY(UtilClass.getMouseWorldPosition(), out int xnewTest, out int ynewTest);
            if (!(xnewTest == x2 && ynewTest == y2))
            {
                //not clicked on same pos


                AcceptedMovePath = false;
                StartCoroutine(handleUnitMove());
                yield break;
            }
            moving = true;
            AcceptedMovePath = false;
            WaitingForAcceptionOfPath = false;


            string outer = "";
            foreach (Vector2Int vec in path)
            {
                outer += vec.ToString() + ",";
            }
            //Debug.Log("Found Path:" + outer);




            yield return StartCoroutine(SelectedUnit.moveToPos(path));
            //selectFriendlyUnit(null);

            Vector2Int[] arr = new Vector2Int[AllUnits.ToArray().Length];
            int index = 0;
            foreach (InGame_Unit_Handler_Script scr in AllUnits)
            {
                arr[index] = scr.getGridPos();
                index++;
            }
            //Debug.Log("hello NewPos");
            LocalView.RPC("GiveMasterNewPositions", Photon.Pun.RpcTarget.MasterClient, Vector2IntArrayToString(arr), PhotonNetwork.LocalPlayer);


        }
        moving = false;
        if (path != null && path.Length != 0)
        {
            foreach (Vector2Int vec in path)
            {
                worldHandler.getBuildLayers().getGridObject(vec.x, vec.y).setPathfindingData(null);
            }
            pathfindingVisualHandler.SetGrid(worldHandler.getBuildLayers());
        }
        yield break;
    }

    [SerializeField] TileBuildData[] pathfindingColors = new TileBuildData[3];
    [SerializeField] pathfindingColorVisualHandler pathfindingVisualHandler;



    public List<Vector2Int> getRestrictedTiles()
    {
        List<Vector2Int> returner = new List<Vector2Int>();
        foreach (Vector2Int[] arr in playerToUnitDictionary.Values)
        {
            foreach (Vector2Int Arrpos in arr)
            {
                returner.Add(Arrpos);
            }
        }
        //Debug.Log("WHYYYYYYY:"+count);
        return returner;
    }

    public bool clickedPosOccupied(Vector2Int pos)
    {
        foreach (Vector2Int[] arr in playerToUnitDictionary.Values)
        {
            foreach (Vector2Int Arrpos in arr)
            {
                if (Arrpos.Equals(pos))
                {
                    return true;
                }
            }
        }
        return false;
    }

    void handleUILeftClick()
    {

    }

    bool IsMouseOverUI()
    {
        return false;
    }





    public Vector3 getPosOnGrid(Vector2Int pos)
    {
        Vector3 newPos = worldHandler.getBuildLayers().getWorldPosition(pos.x, pos.y);
        newPos.x = newPos.x + worldHandler.getBuildLayers().getCellSize() / 2;
        newPos.y = newPos.y + worldHandler.getBuildLayers().getCellSize() / 2;
        return newPos;
    }





    //network unit pos storage handling
    [SerializeField]Dictionary<Photon.Realtime.Player, Vector2Int[]> playerToUnitDictionary = new Dictionary<Photon.Realtime.Player, Vector2Int[]>();
    [PunRPC]
    void GiveMasterNewPositions(string arrAsString, Photon.Realtime.Player plr)
    {
        playerToUnitDictionary.Remove(plr);
        Vector2Int[] temp = stringToVector2Int(arrAsString);
        bool removedAllEnemyUnitsOfASinglePlayer = false;
        if (temp.Length > 0)
        {
            playerToUnitDictionary.Add(plr, stringToVector2Int(arrAsString));
        }
        else
        {
            removedAllEnemyUnitsOfASinglePlayer = true;
        }
        resetShooting();
        LocalView.RPC("giveClientPlayerPositions", Photon.Pun.RpcTarget.Others, arrAsString, plr);
        if (removedAllEnemyUnitsOfASinglePlayer)
        {

            handleWinCondition();
        }
    }

    [PunRPC]
    void clientSetDictionary(Dictionary<Photon.Realtime.Player, Vector2Int[]> playerToUnitDictionary)
    {
        this.playerToUnitDictionary = playerToUnitDictionary;
    }

    [PunRPC]
    void clientClearDictionary()
    {
        playerToUnitDictionary.Clear();
    }

    [PunRPC]
    void giveClientPlayerPositions(string asString, Photon.Realtime.Player plr)
    {
        playerToUnitDictionary.Remove(plr);
        Vector2Int[] temp = stringToVector2Int(asString);
        if (temp.Length > 0)
        {
            playerToUnitDictionary.Add(plr, stringToVector2Int(asString));
        }
        resetShooting();
    }

    private string Vector2IntArrayToString(Vector2Int[] arr)
    {
        string returner = "";
        for (int i = 0; i < arr.Length; i++)
        {
            returner += arr[i].x + "," + arr[i].y;
            if (i != arr.Length - 1)
            {
                returner += ";";
            }
        }

        //Debug.Log("Vector2IntArrayToString returner:" + returner);
        return returner;
    }

    private Vector2Int[] stringToVector2Int(string s)
    {
        if (s.Length == 0)
        {
            return new Vector2Int[0];
        }
        string[] values = s.Split(';');
        //Debug.Log("s:" + s + "values length:" + values.Length);
        Vector2Int[] returner = new Vector2Int[values.Length];
        int index = 0;
        foreach (string str in values)
        {
            string[] data = str.Split(',');
            //Debug.Log("vectorarr data Length:"+data.Length);
            returner[index] = new Vector2Int(int.Parse(data[0]), int.Parse(data[1]));

            index++;
        }
        return returner;
    }

    public Vector2 getClosestTilePos(Vector2 pos)
    {
        worldHandler.getBuildLayers().GetXY(pos, out int x, out int y);
        return getPosOnGrid(new Vector2Int(x, y));
    }



    List<InGame_Unit_Handler_Script> getAllSeenUnits()
    {
        List<InGame_Unit_Handler_Script> allSeenUnits = new List<InGame_Unit_Handler_Script>();
        foreach (InGame_Unit_Handler_Script scr in AllUnits)
        {
            allSeenUnits.AddRange(scr.getSeenEnemyUnits());
        }
        return allSeenUnits;
    }

    //shooting stuff

    public InGame_Unit_Handler_Script selectedEnemyUnit = null;
    [SerializeField] bool allowShootingFromAnyOrJustSelf = true;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] LayerMask friendlyUnitLayer;
    //true = allow clicking on any unit seen, false = only allow clikcing on units seen by the selected friendly unit

    InGame_Unit_Handler_Script originallySelectedEnemy = null;
    Vector2 bulletTargetPos;
    private void HandleClickOnEnemyUnit()
    {
        if (selectedEnemyUnit == null)
        {
            resetShooting();
            return;
        }

        if (originallySelectedEnemy != null && originallySelectedEnemy == selectedEnemyUnit)
        {
            //clicked same unit to confirm choice
            //Debug.Log("SHOOT!");
            handleShoot();
        }
        else
        {
            originallySelectedEnemy = selectedEnemyUnit;
            //Debug.Log("Show Bullet Path");




            bool blocked = false;
            List<InGame_Unit_Handler_Script> allSeenUnits = getAllSeenUnits();
            RaycastHit2D hit = Physics2D.Raycast(SelectedUnit.gameObject.transform.position, (selectedEnemyUnit.gameObject.transform.position - SelectedUnit.gameObject.transform.position), Vector2.Distance(SelectedUnit.gameObject.transform.position, selectedEnemyUnit.gameObject.transform.position), ~friendlyUnitLayer);
            Vector3 endPos = new Vector3();
            if (hit.collider == null)
            {
                Debug.Log("WTF How Did I Not Hit Anything??????");
            }
            else
            {
                Debug.Log("I Hit: name:" + hit.collider.gameObject.name + " | tag:" + hit.collider.gameObject.tag);
                if (hit.collider.gameObject.tag.Equals(selectedEnemyUnit.gameObject.tag))
                {
                    //direct line of sight to Enemy
                    //Debug.Log("Enemy Los");
                    endPos = selectedEnemyUnit.gameObject.transform.position;
                }
                else
                {
                    //line of sight to enemy blocked
                    //Debug.Log("Blocked Los");
                    blocked = true;
                    endPos = hit.point;
                }
            }
            if (SelectedUnit.getSeenEnemyUnits().Contains(selectedEnemyUnit))
            {
                bulletTargetPos = endPos;
                //Debug.Log("I Click Enemy Unit Seen By Selected");
                //clicked on a unit seen by Selected friendly
                if (moving || WaitingForAcceptionOfPath)
                {
                    CancelMoveRequest = true;
                    resetPathVisualGrid();
                }
                if (blocked)
                {
                    lineRenderer.startColor = Color.red;
                    lineRenderer.endColor = Color.red;
                }
                else
                {
                    lineRenderer.startColor = Color.green;
                    lineRenderer.endColor = Color.green;
                }
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, SelectedUnit.gameObject.transform.position);
                lineRenderer.SetPosition(1, endPos);
            }
            else if (allowShootingFromAnyOrJustSelf && allSeenUnits.Contains(selectedEnemyUnit))
            {
                bulletTargetPos = endPos;
                //Debug.Log("I Click Enemy Unit Seen By Friendly");
                //clicked on a unit seen by any friendly
                if (moving || WaitingForAcceptionOfPath)
                {
                    CancelMoveRequest = true;
                    resetPathVisualGrid();
                }
                if (SelectedUnit.getSeenEnemyUnitsFar().Contains(selectedEnemyUnit))
                {
                    //seen by self but far
                    lineRenderer.startColor = Color.yellow;
                    lineRenderer.endColor = Color.yellow;
                }
                else
                {
                    //only seen by others
                    lineRenderer.startColor = Color.red;
                    lineRenderer.endColor = Color.red;
                }
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, SelectedUnit.gameObject.transform.position);
                lineRenderer.SetPosition(1, endPos);
            }
        }
    }


    void resetShooting()
    {
        lineRenderer.positionCount = 0;
        originallySelectedEnemy = null;
        selectedEnemyUnit = null;
    }

    void handleShoot()
    {
        resetShooting();
        if (lineRenderer.startColor == Color.green)
        {
            Debug.Log("Make New Bullet");//
            GameObject bullet = PhotonNetwork.Instantiate("Bullet", SelectedUnit.gameObject.transform.position, Quaternion.identity);
            bullet.GetComponent<Bullet_Handler_Script>().setTarget(SelectedUnit.gameObject.transform.position, bulletTargetPos, false);
        }
        else if (lineRenderer.startColor == Color.yellow)
        {
            //Debug.Log("Make New Bullet Far");
            GameObject bullet = PhotonNetwork.Instantiate("Bullet", SelectedUnit.gameObject.transform.position, Quaternion.identity);
            bullet.GetComponent<Bullet_Handler_Script>().setTarget(SelectedUnit.gameObject.transform.position, bulletTargetPos, true);
        }
    }


    //win lose handling

    [SerializeField] GameObject blackScreen;
    bool loseConditionHandled = false;
    void handleAllFriendlyUnitsDead()
    {
        blackScreen.layer = LayerMask.NameToLayer("Mask");
        SpriteRenderer sr = blackScreen.GetComponent<SpriteRenderer>();
        Color c = sr.color;
        c.a = 0;
        sr.color = c;

    }

    void handleWinCondition()
    {
        if (playerToUnitDictionary.Keys.Count == 1)
        {
            List<Photon.Realtime.Player> winner = new List<Photon.Realtime.Player>(playerToUnitDictionary.Keys);
            Debug.Log(winner[0] + " Is The Winner!");
        }
    }

}
