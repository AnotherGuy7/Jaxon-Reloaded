using Godot;
using System;

public class PlayerUI : Control
{
	[Export]
	public Texture[] healthBarTexture;

	private const float DefaultHealthTextureScale = 3f;

	private TextureRect healthBar;
	private TextureRect[] healthBarTicks;
	private Label moneyLabel;
	public static Panel bossHealthBarPanel;
	public static TextureRect bossHealthTexture;

	public static PlayerUI playerUI;

	public override void _Ready()
	{
		healthBar = GetNode<TextureRect>("UILayer/HealthBar/HealthBar");
		healthBarTicks = new TextureRect[Player.MaxHealth[Player.healthLevel - 1]];
		for (int i = 0; i < Player.MaxHealth[Player.healthLevel - 1]; i++)
			healthBarTicks[i] = healthBar.GetChild(i) as TextureRect;
			
		moneyLabel = GetNode<Label>("UILayer/Money/MoneyLabel");
		bossHealthBarPanel = GetNode<Panel>("UILayer/BossHealthPanel");
		bossHealthTexture = GetNode<TextureRect>("UILayer/BossHealthPanel/BossHealthTexture");

		healthBar.Texture = healthBarTexture[Player.healthLevel - 1];
		playerUI = this;
	}

	public override void _Process(float delta)
	{
		for (int i = 0; i < Player.MaxHealth[Player.healthLevel - 1]; i++)
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

		for (int i = 0; i < Player.MaxHealth[Player.healthLevel - 1]; i++)
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

	public static void BossHealthBarVisibilty(bool hidden)		//Just for cleansliness
	{
		bossHealthBarPanel.Visible = !hidden;
	}

	public static void UpdateBossHealthScale(float percentage)
	{
		float newScale = percentage * DefaultHealthTextureScale;
		bossHealthTexture.RectScale = new Vector2(newScale, DefaultHealthTextureScale);
	}

	public static void SetMoneyCounterModulate(Color newColor)
    {
		playerUI.moneyLabel.Modulate = newColor;
    }
}
