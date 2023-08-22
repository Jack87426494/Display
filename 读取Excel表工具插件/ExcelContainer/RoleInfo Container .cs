using System.Collections.Generic;
[System.Serializable]
public class RoleInfoContainer:IContainer
{
  public Dictionary<int,RoleInfo> dic = new Dictionary<int,RoleInfo>();
}