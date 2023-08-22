using System.Collections.Generic;
[System.Serializable]
public class ZombieInfoContainer:IContainer
{
  public Dictionary<int,ZombieInfo> dic = new Dictionary<int,ZombieInfo>();
}