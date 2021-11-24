using Godot;
using System;

public class PlayerUI : Control
{
	[Export]
	public Texture healthBarTexture;

	private readonly Vector2 healthBarStartPoint = new Vector2(6.5f, 9f);

	private TextureRect healthBar;
	private TextureRect[] healthBarTicks;
	private Label moneyLabel;

	public override void _Ready()
	{
		healthBar = GetNode<TextureRect>("UILayer/HealthBar/HealthBar");
		healthBarTicks = new TextureRect[Player.MaxHealth];
		for (int i = 0; i < Player.MaxHealth; i++)
			healthBarTicks[i] = healthBar.GetChild(i) as TextureRect;
			
			moneyLabel = GetNode<Label>("UILayer/Money/MoneyLabel");

	}

	public override void _Process(float delta)
	{
		for (int i = 0; i < Player.MaxHealth; i++)
		{
			healthBarTicks[i].Visible = false;
			if (i + 1 <= Player.playerHealth)
			{
				healthBarTicks[i].Visible = true;
			}
		}
		
		moneyLabel.Text = Player.playerMoney.ToString();
	}

	/*private void DrawHealthBarTicks(Control node)		//Draws are just busted
	{
		if (!IsInstanceValid(node))
			return;

		for (int i = 0; i < Player.MaxHealth; i++)
		{
			if (i < Player.playerHealth)
			{
				Vector2 barPosition = healthBarStartPoint + new Vector2(i * 22f, 0f);
				Vector2 barSize = new Vector2(25, 16) * 2f;
				Rect2 barRect = new Rect2(barPosition, barSize);
				node.DrawTextureRect(healthBarTexture, barRect, false);
			}
		}
	}*/
}
