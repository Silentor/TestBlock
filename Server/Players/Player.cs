using System;
using Microsoft.Xna.Framework;
using Silentor.TB.Common.Maps.Geometry;
using Silentor.TB.Server.Maps;
using Silentor.TB.Server.Tools;

namespace Silentor.TB.Server.Players
{
    public interface IPlayer
    {
        int Id { get; }

        Vector3 Position { get; }

        Quaternion Rotation { get; }

        bool IsPositionChanged { get; }

        bool IsRotationChanged { get; }

        event Action<Vector3> PositionChanged;
    }

    public class Player : IPlayer
    {
        public int Id { get; private set; }

        public string Name { get; private set; }

        public Vector3 Position { get; private set; }

        public Quaternion Rotation { get; private set; }

        public bool IsPositionChanged { get; private set; }

        public bool IsRotationChanged { get; private set; }

        public event Action<Vector3> PositionChanged;

        public Sensor Sensor { get; private set; }

        public Player(int id, string name, Map map, Time.Timer timer, Vector3 startPosition, World world)
        {
            Id = id;
            Name = name;
            Position = startPosition;
            _map = map;
            Sensor = new Sensor(map, world, 16, timer, this);
        }

        public void SetAcceleration(Vector3 accel)
        {
            if (accel != Vector3.Zero)
            {
                accel = Vector3.Transform(accel, _rotation);
                accel.Normalize();
                _acceleration = accel * AccelerationValue;
                _isStopMode = false;
            }
            else
            {
                _acceleration = Vector3.Zero;
                _isStopMode = true;
            }
        }

        public void SetRotation(Vector2 deltaRotation)
        {
            _degreesRotation += deltaRotation;

            if (_degreesRotation.Y > 90) _degreesRotation.Y = 90;
            if (_degreesRotation.Y < -90) _degreesRotation.Y = -90;
            _degreesRotation.X = Angles.Wrap(_degreesRotation.X);

            _rotation = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(_degreesRotation.X),
                MathHelper.ToRadians(-_degreesRotation.Y), 0);
        }

        public void Jump()
        {
            if (!_isFreeFall)
                _jump = true;
        }

        /// <summary>
        /// Simulate one tick of player's physics
        /// </summary>
        public void Simulate()
        {
            var blockPos = Position.ToMapPosition();
            var currentChunk = _map.GetChunk(Chunk.ToChunkPosition(blockPos));

            if (currentChunk == null) return;

            //Calc G-accel
            var localBlockPos = Chunk.ToLocalPosition(blockPos);
            if (Chunk.IsCorrectLocalPosition(localBlockPos + Vector3i.Down) && currentChunk.GetBlock(localBlockPos + Vector3i.Down).IsEmpty)
            {
                _isFreeFall = true;
            }
            else
            {
                if (_isFreeFall)
                {
                    _freeFallVelocity = Vector3.Zero;
                    _isFreeFall = false;
                    var y = blockPos.ToObjectPosition().Y;
                    Position = new Vector3(Position.X, y, Position.Z);
                }
            }

            if (_isFreeFall)
            {
                var newVelocity = _freeFallVelocity + GAcceleration*Time.Timer.DeltaTime;
                _freeFallVelocity = newVelocity.ClampMagnitude(MaxGVelocity);
            }
            else
            {
                //Calc move velocity
                if (_isStopMode)
                {
                    if (_velocity.Length() > AccelerationValue*Time.Timer.DeltaTime)
                        _velocity = _velocity - Vector3.Normalize(_velocity) * AccelerationValue * Time.Timer.DeltaTime;
                    else
                    {
                        _velocity = Vector3.Zero;
                        _isStopMode = false;
                    }
                }
                else
                {
                    var newVelocity = _velocity + _acceleration*Time.Timer.DeltaTime;
                    _velocity = newVelocity.ClampMagnitude(MaxVelocity);
                }

                if (_jump)
                {
                    _jump = false;
                    _freeFallVelocity += JumpVelocity;
                }
            }

            Position += (_velocity + _freeFallVelocity) * Time.Timer.DeltaTime;

            //Position dirty flag
            if (Position != _lastPosition)
            {
                _lastPosition = Position;
                IsPositionChanged = true;
                //Position event
                DoPositionChanged();
            }
            else
                IsPositionChanged = false;

            Rotation = _rotation;

            //Rotation dirty flag
            if (Rotation != _lastRotation)
            {
                _lastRotation = Rotation;
                IsRotationChanged = true;
            }
            else
                IsRotationChanged = false;
        }

        private readonly Map _map;
        private static readonly Vector3 GAcceleration = new Vector3(0, -9.8f, 0);
        private Vector3 _acceleration = Vector3.Zero;
        private Vector3 _velocity;
        private Vector3 _lastPosition;
        private bool _isStopMode;
        private bool _isFreeFall;
        private Vector3 _freeFallVelocity;
        private bool _jump;
        private Quaternion _rotation = Quaternion.Identity;
        private Vector2 _degreesRotation;
        private Quaternion _lastRotation = Quaternion.Identity;

        private const float AccelerationValue = 5;
        private const float MaxVelocity = 5;
        private const float MaxGVelocity = 20;
        private static readonly Vector3 JumpVelocity = new Vector3(0, 5, 0);

        private void DoPositionChanged()
        {
            var handler = PositionChanged;
            if (handler != null) handler(Position);
        }

        public override string ToString()
        {
            return String.Format("Player id {0}, name {1}", Id, Name);
        }
    }
}
