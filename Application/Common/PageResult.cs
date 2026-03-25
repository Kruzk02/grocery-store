namespace Application.Common;

public record PageResult<T>(int Total, List<T> Data);
