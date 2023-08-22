using System.Collections.Generic;
[System.Serializable]
public class SceneInfoContainer:IContainer
{
  public Dictionary<int,SceneInfo> dic = new Dictionary<int,SceneInfo>();
}