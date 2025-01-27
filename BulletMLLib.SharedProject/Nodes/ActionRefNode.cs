using System;

namespace BulletMLLib.SharedProject.Nodes
{
	/// <summary>
	/// Action reference node.
	/// This node type references another Action node.
	/// </summary>
	public partial class ActionRefNode : ActionNode
	{
		#region Members

		public ActionNode ReferencedActionNode { get; private set; }

		#endregion //Members

		#region Methods

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionRefNode"/> class.
		/// </summary>
		public ActionRefNode() : base(ENodeName.actionRef)
		{
		}

		/// <summary>
		/// Validates the node.
		/// Overloaded in child classes to validate that each type of node follows the correct business logic.
		/// This checks stuff that isn't validated by the XML validation
		/// </summary>
		public override void ValidateNode()
		{
			//do any base class validation
			base.ValidateNode();

			//Find the action node this dude references
			var refNode = GetRootNode().FindLabelNode(Label, ENodeName.action);

			//make sure we foud something
			if (null == refNode)
			{
				throw new NullReferenceException("Couldn't find the action node \"" + Label + "\"");
			}

			ReferencedActionNode = refNode as ActionNode;
			if (null == ReferencedActionNode)
			{
				throw new NullReferenceException("The BulletMLNode \"" + Label + "\" isn't an action node");
			}
		}

		#endregion //Methods
	}
}
