import React, { Component } from "react";

class UserStoreEntry extends Component {
  state = {};
  render() {
    return (
      <tr>
        <td>{this.props.storeName}</td>
        <td> {this.props.description} </td>
        <td>
          <a
            href={
              this.props.guid ? `/store/${this.props.guid}` : this.setState({})
            }
            className="d-flex p-1 justify-content-center btn btn-outline-primary"
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
