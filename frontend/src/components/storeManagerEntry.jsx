import React, { Component } from "react";
import { Checkbox } from "@material-ui/core";

class StoreManagementEntry extends Component {
  state = {};
  render() {
    return (
      <tr>
        <td>{this.props.username}</td>
        <td>
          <Checkbox
            color="primary"
            onChange={(event) => event.preventDefault()}
            checked={this.props.permissions.canAddProduct}
          />
        </td>
        <td>
          <Checkbox
            color="primary"
            onChange={(event) => event.preventDefault()}
            checked={this.props.permissions.canDeleteProduct}
          />{" "}
        </td>
        <td>
          <Checkbox
            color="primary"
            onChange={(event) => event.preventDefault()}
            checked={this.props.permissions.canUpdateProduct}
          />{" "}
        </td>
        <td>
          <Checkbox
            color="primary"
            onChange={(event) => event.preventDefault()}
            checked={this.props.permissions.canGetSellersInfo}
          />{" "}
        </td>
        {this.props.isSubordinate ? (
          <td>
            <button className="d-flex p-1 justify-content-center btn btn-outline-primary">
              {" "}
              Edit Permissions{" "}
            </button>{" "}
          </td>
        ) : null}
        {this.props.isSubordinate ? (
          <td>
            <button className="d-flex p-1 justify-content-center btn btn-outline-primary">
              {" "}
              Remove Manager{" "}
            </button>{" "}
          </td>
        ) : null}
      </tr>
    );
  }
}

export default StoreManagementEntry;
