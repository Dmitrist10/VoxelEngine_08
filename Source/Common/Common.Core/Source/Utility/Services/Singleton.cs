namespace VoxelEngine.Core;

/// <summary>
///  Base class for making a Singleton pattern
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T> where T : Singleton<T>, new()
{
  public static T instance = new T();
}
