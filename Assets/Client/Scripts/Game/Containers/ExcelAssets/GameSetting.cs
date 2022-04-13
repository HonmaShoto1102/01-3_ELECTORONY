using Client.Game.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class GameSetting : ScriptableObject
{
	public List<GameSettingModel> Sheet1; // Replace 'EntityType' to an actual type that is serializable.
}
