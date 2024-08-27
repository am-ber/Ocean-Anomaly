
public interface Observer<I1, I2>
{
    public void OnNotify(I1 caller, I2 input);
}
public interface Observer<I>
{
    public void OnNotify(I input);
}
public interface Observer
{
    public void OnNotify();
}