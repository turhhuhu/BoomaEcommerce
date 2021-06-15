import React, { Component } from "react";
import { connect } from "react-redux";
import { seeNotification } from "../actions/userActions";

class NotificationEntry extends Component {
  state = {};

  handleMouseHoverNotification = () => {
    if (!this.props.wasSeen) {
      this.props.dispatch(seeNotification(this.props.guid));
    }
  };

  getTypeMessage = () => {
    if (this.props.type === "roleDismissalNotification") {
      return "Role dismissal";
    } else if (this.props.type === "storePurchaseNotification") {
      return "Store purchase";
    } else if (this.props.type === "newOfferNotification") {
      return "New offer";
    } else if (this.props.type === "offerDeclinedNotification") {
      return "Offer declined";
    } else if (this.props.type === "offerApprovedNotification") {
      return "Offer approved";
    }

    return "Regular";
  };

  render() {
    return (
      <tr
        onMouseOut={this.handleMouseHoverNotification}
        className={this.props.wasSeen ? "table-info" : "table-warning"}
      >
        <td>{this.getTypeMessage()}</td>
        <td>{this.props.message}</td>
      </tr>
    );
  }
}

export default connect()(NotificationEntry);
