namespace DotAigent.Core;
public interface ITool { string Name { get; } Task<object> ExecuteAsync(IDictionary<string, object> parameters); }
/*public interface ITool*/
/*{*/
/*    public Task<T> ExecuteAsync<T>();*/
/**/
/*}*/
