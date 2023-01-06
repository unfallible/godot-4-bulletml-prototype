using System.Diagnostics;
using BulletMLLib.SharedProject.Nodes;
using Godot;

namespace BulletMLLib.SharedProject.Tasks
{
	/// <summary>
	/// This task adds acceleration to a bullet.
	/// </summary>
	public partial class AccelTask : BulletMLTask
	{
		#region Members

		/// <summary>
		/// How long to run this task... measured in frames
		/// </summary>
		public float Duration { get; private set; }

		/// <summary>
		/// The direction to accelerate in 
		/// </summary>
		private Vector2 _acceleration = Vector2.Zero;
		
		/// <summary>
		/// Gets or sets the acceleration.
		/// </summary>
		/// <value>The acceleration.</value>
		public Vector2 Acceleration 
		{ 
			get
			{
				return _acceleration;
			}
			private set
			{
				_acceleration = value;
			}
		}

		#endregion //Members

		#region Methods

		/// <summary>
		/// Initializes a new instance of the <see cref="BulletMLTask"/> class.
		/// </summary>
		/// <param name="node">Node.</param>
		/// <param name="owner">Owner.</param>
		public AccelTask(AccelNode node, BulletMLTask owner) : base(node, owner)
		{
			Debug.Assert(null != Node);
			Debug.Assert(null != Owner);
		}

		/// <summary>
		/// this sets up the task to be run.
		/// </summary>
		/// <param name="bullet">Bullet.</param>
		protected override void SetupTask(Bullet bullet)
		{
			//set the accelerataion we are gonna add to the bullet
			Duration = Node.GetChildValue(ENodeName.term, this, bullet);

			//check for divide by 0
			if (0.0f == Duration)
			{
				Duration = 1.0f;
			}

			//Get the horizontal node
			var horiz = Node.GetChild(ENodeName.horizontal) as HorizontalNode;
			if (null != horiz)
			{
				//Set the x component of the acceleration
				switch (horiz.NodeType)
				{
					case ENodeType.sequence:
					{
						//Sequence in an acceleration node means "add this amount every frame"
						_acceleration.x = horiz.GetValue(this, bullet);
					}
					break;

					case ENodeType.relative:
					{
						//accelerate by a certain amount
						_acceleration.x = horiz.GetValue(this, bullet) / Duration;
					}
					break;

					default:
					{
						//accelerate to a specific value
						_acceleration.x = (horiz.GetValue(this, bullet) - bullet.Acceleration.x) / Duration;
					}
					break;
				}
			}

			//Get the vertical node
			var vert = Node.GetChild(ENodeName.vertical) as VerticalNode;
			if (null != vert)
			{
				//set teh y component of the acceleration
				switch (vert.NodeType)
				{
					case ENodeType.sequence:
					{
						//Sequence in an acceleration node means "add this amount every frame"
						_acceleration.y = vert.GetValue(this, bullet);
					}
					break;

					case ENodeType.relative:
					{
						//accelerate by a certain amount
						_acceleration.y = vert.GetValue(this, bullet) / Duration;
					}
					break;

					default:
					{
						//accelerate to a specific value
						_acceleration.y = (vert.GetValue(this, bullet) - bullet.Acceleration.y) / Duration;
					}
					break;
				}
			}
		}

		/// <summary>
		/// Run this task and all subtasks against a bullet
		/// This is called once a frame during runtime.
		/// </summary>
		/// <returns>ERunStatus: whether this task is done, paused, or still running</returns>
		/// <param name="bullet">The bullet to update this task against.</param>
		public override ERunStatus Run(Bullet bullet)
		{
			//Add the acceleration to the bullet
			bullet.Acceleration += Acceleration;

			//decrement the amount if time left to run and return End when this task is finished
			Duration -= 1.0f * bullet.TimeSpeed;
			if (Duration <= 0.0f)
			{
				TaskFinished = true;
				return ERunStatus.End;
			}
			else 
			{
				//since this task isn't finished, run it again next time
				return ERunStatus.Continue;
			}
		}

		#endregion //Methods
	}
}