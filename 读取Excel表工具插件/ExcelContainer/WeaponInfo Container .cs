using System.Collections.Generic;
[System.Serializable]
public class WeaponInfoContainer:IContainer
{
  public Dictionary<int,WeaponInfo> dic = new Dictionary<int,WeaponInfo>();
}