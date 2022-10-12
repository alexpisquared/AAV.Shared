﻿namespace CI.Standard.Lib.Base;

public class UserSettingsStore
{
  protected static void Save<T>(T ths) => JsonFileSerializer.Save(ths, FullPath);                  //JsonIsoFileSerializer.Save(ths, iss: IsoConst.URoaA);
  protected static T Load<T>() where T : new() => JsonFileSerializer.Load<T>(FullPath) ?? new T(); //JsonIsoFileSerializer.Load<T>(iss: IsoConst.URoaA) ?? new T();
  protected DateTimeOffset LastSave { get; set; } = DateTimeOffset.MinValue;

  public static string FullPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @$"AppSettings\{AppDomain.CurrentDomain.FriendlyName}\UserSettings.json");
}