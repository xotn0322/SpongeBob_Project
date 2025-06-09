using UnityEngine;

public class ResourcesManager : IEngineComponent
{
    #region Interface
    public static ResourcesManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ResourcesManager();
            }

            return _instance;
        }
    }
    private static ResourcesManager _instance;

    public IEngineComponent Init()
    {
        CheckPathExist();
        return this;
    }
    #endregion

    private void CheckPathExist()
    {
        var pathExist = IOUtil.DirectoryExist(Constant.Path.DEFAULT_RESOURCES_PATH);
        if (pathExist == false)
        {
            Debug.LogError($"ResourcesManager : Error in CheckPathExist. Resources Path Doesn't Exists. path={Constant.Path.DEFAULT_RESOURCES_PATH}");
        }
    }

    public T Load<T>(string path) where T : UnityEngine.Object
    {
        T resource = Resources.Load<T>(path);
        if (resource == null)
        {
            Debug.LogError($"ResourcesManager : Error in Load. Fail To Load Resource. path={IOUtil.CombinePath(Constant.Path.DEFAULT_RESOURCES_PATH, path)}. type={typeof(T)}");
        }

        return resource;
    }
}
