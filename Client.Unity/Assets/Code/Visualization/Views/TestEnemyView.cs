using System;
using ModestTree.Zenject;
using Silentor.TB.Client.Players;
using UnityEngine;
using UnityEngine.UI;

namespace Silentor.TB.Client.Visualization.Views
{
    /// <summary>
    /// Unity representation of another player (Enemy)
    /// </summary>
    public class TestEnemyView : MonoBehaviour, IActor
    {
        [Inject]
        public IActor Enemy;

        public Text NameLabel;

        public int Id { get { return Enemy.Id; }}

        public Vector3 Position { get { return Enemy.Position; } }

        public Quaternion Rotation { get { return Enemy.Rotation; } }

        [PostInject]
        public void Init()
        {
            transform.position = Position;
            transform.parent = null;

            NameLabel.text = "Player " + Enemy.Id;

            Enemy.Moved += () => { transform.position = Position; };
            Enemy.Rotated += () => { transform.rotation = Rotation; };
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }

        public event Action Moved;
        public event Action Rotated;
    }
}
