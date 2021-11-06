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

    [SerializeField]
    protected Transform baseTransform;

    public Vector3 BasePosition { get => this.baseTransform.position; }

    public event System.Action<LevelManager> OnLevelComplete;

    private void Start()
    {
        this.bricks.OnAllBricksDestroyed += HandleAllBricksDestroyed;
    }

    public void Init(Bricks bricks, Ceiling ceiling, Light firstPlanLight, CinemachineVirtualCamera virtCamera, Transform baseTransform)
    {
        this.bricks = bricks;
        this.ceiling = ceiling;
        this.firstPlanLight = firstPlanLight;
        this.virtCamera = virtCamera;
        this.baseTransform = baseTransform;
    }

    public void InitLevel()
    {
        this.firstPlanLight.gameObject.SetActive(false);
        this.virtCamera.gameObject.SetActive(false);
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
        this.ceiling.DestroyCeiling();
        this.OnLevelComplete?.Invoke(this);
    }

    public void AddHealing()
    {
        this.bricks.AddHealing();
    }

}
