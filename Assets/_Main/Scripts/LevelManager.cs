using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Sirenix.OdinInspector;

public class LevelManager : MonoBehaviour
{

    [SerializeField]
    protected Bricks bricks;

    [SerializeField]
    protected Ceiling ceiling;

    [SerializeField]
    protected Light firstPlanLight;

    [SerializeField]
    protected CinemachineVirtualCamera virtCamera;

    public event System.Action<LevelManager> OnLevelComplete;

    private void Start()
    {
        this.firstPlanLight.gameObject.SetActive(false);
        this.virtCamera.gameObject.SetActive(false);
        this.bricks.OnAllBricksDestroyed += HandleAllBricksDestroyed;
    }

    public void Init(Bricks bricks, Ceiling ceiling, Light firstPlanLight, CinemachineVirtualCamera virtCamera)
    {
        this.bricks = bricks;
        this.ceiling = ceiling;
        this.firstPlanLight = firstPlanLight;
        this.virtCamera = virtCamera;
    }

    public void StartLevel()
    {
        this.firstPlanLight.gameObject.SetActive(true);
        this.virtCamera.gameObject.SetActive(true);
    }

    public void EndLevel()
    {
        this.firstPlanLight.gameObject.SetActive(false);
        this.virtCamera.gameObject.SetActive(false);
    }

    protected void HandleAllBricksDestroyed()
    {
        this.OnLevelComplete?.Invoke(this);
    }

}
