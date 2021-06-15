import React, { Component } from "react";
import { Checkbox } from "@material-ui/core";
import {
  fetchStoreRoles,
  fetchStoreSubordinates,
  removeStoreManager,
} from "../actions/storeActions";
import { connect } from "react-redux";
import EditStoreManagerPermissions from "./editStoreManagerPermissions";

class StoreManagementEntry extends Component {
  state = { isDialogOpen: false };

  handleEditPermissions = () => {
    this.setState({ isDialogOpen: true });
  };

  closeEditPermissionsDialog = () => {
    this.setState({ isDialogOpen: false });
  };

  handleRemoveManager = () => {
    if (this.props.isAuthenticated) {
      this.props
        .dispatch(removeStoreManager(this.props.myRole?.guid, this.props.guid))
        .then((success) => {
          if (success) {
            this.props.dispatch(
              fetchStoreRoles(this.props.myRole?.storeMetaData.storeGuid)
            );
            if (this.props.myRole?.type === "ownership") {
              this.props.dispatch(
                fetchStoreSubordinates(
                  this.props.myRole?.storeMetaData.storeGuid,
                  this.props.myRole?.guid
                )
              );
            }
          }
        });
    }
  };

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
        <EditStoreManagerPermissions
          storeGuid={this.props.storeGuid}
          managementGuid={this.props.guid}
          management={this.props.management}
          isDialogOpen={this.state.isDialogOpen}
          closeDialog={this.closeEditPermissionsDialog}
        />
        {this.props.isSubordinate ? (
          <td>
            <button
              onClick={this.handleEditPermissions}
              className="d-flex p-1 justify-content-center btn btn-outline-primary"
            >
              {" "}
              Edit Permissions{" "}
            </button>{" "}
          </td>
        ) : null}
        {this.props.isSubordinate ? (
          <td>
            <button
              onClick={this.handleRemoveManager}
              className="d-flex p-1 justify-content-center btn btn-outline-primary"
            >
              {" "}
              Remove Manager{" "}
            </button>{" "}
          </td>
        ) : null}
      </tr>
    );
  }
}
const mapStateToProps = (store) => {
  return {
    isAuthenticated: store.auth.isAuthenticated,
    myRole: store.user.storeRole,
  };
};
export default connect(mapStateToProps)(StoreManagementEntry);
