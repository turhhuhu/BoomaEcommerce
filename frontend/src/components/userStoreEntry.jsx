import React, { Component } from "react";

class UserStoreEntry extends Component {
  state = {};
  render() {
    return (
      <tr>
        <td>
          <p className="title mb-0"> {this.props.storeName} </p>
        </td>
        <td> {this.props.description} </td>
        <td>
          <a
            href={`/store/${this.props.guid}`}
            className="btn btn-outline-primary"
          >
            {" "}
            Details{" "}
          </a>{" "}
        </td>
      </tr>
    );
  }
}

export default UserStoreEntry;
