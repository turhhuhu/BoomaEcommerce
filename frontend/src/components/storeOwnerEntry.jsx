import React, { Component } from "react";

class StoreOwnerEntry extends Component {
  state = {};
  render() {
    return (
      <tr>
        <td>{this.props.username}</td>
        {this.props.isSubordinate ? (
          <td>
            <button className="d-flex p-1 justify-content-center btn btn-outline-primary">
              {" "}
              Remove Owner{" "}
            </button>{" "}
          </td>
        ) : null}
      </tr>
    );
  }
}

export default StoreOwnerEntry;
