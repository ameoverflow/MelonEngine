using Chroma.Graphics;

public interface Scene
{
    public void Draw(RenderContext context);
    public void Update(float delta);
    public void FixedUpdate(float delta);
    public void Init();
}