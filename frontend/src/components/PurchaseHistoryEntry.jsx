import React, { Component } from "react";

class PurchaseHistoryEntry extends Component {
  state = {};
  render() {
    return (
      <tr>
        <td>
          <strong className="title mb-0">
            Price: ${this.props.purchase.totalPrice}{" "}
          </strong>
        </td>
        <td>
          <div className="float-right">
            <a
              href={`${this.props.hrefPrefix} + /${this.props.purchase.guid}`}
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
