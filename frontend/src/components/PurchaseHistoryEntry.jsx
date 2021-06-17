import React, { Component } from "react";

class PurchaseHistoryEntry extends Component {
  state = {};
  render() {
    return (
      <tr>
        <td>
          <strong className="title mb-0">
            buyer:{" "}
            {this.props.purchase.userMetaData
              ? this.props.purchase.userMetaData.userName.startsWith("GUEST")
                ? "guest"
                : this.props.purchase.userMetaData.userName
              : this.props.purchase.buyer.userName.startsWith("GUEST")
              ? "guest"
              : this.props.purchase.buyer.userName}{" "}
          </strong>
        </td>
        <td>
          <strong className="title mb-0">
            Price: ${this.props.purchase.totalPrice}{" "}
          </strong>
        </td>
        <td>
          <div className="float-right">
            <a
              href={`${this.props.hrefPrefix}/${this.props.purchase.guid}`}
              className="btn btn-outline-primary"
            >
              {" "}
              Details{" "}
            </a>
          </div>
        </td>
      </tr>
    );
  }
}

export default PurchaseHistoryEntry;
