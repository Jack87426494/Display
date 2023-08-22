using System.Collections.Generic;
[System.Serializable]
public class TowerInfoContainer:IContainer
{
  public Dictionary<int,TowerInfo> dic = new Dictionary<int,TowerInfo>();
}