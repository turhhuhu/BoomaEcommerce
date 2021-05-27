import React, { Component } from "react";
import { connect } from "react-redux";
import {
  fetchStoreRoles,
  fetchStoreSubordinates,
  removeStoreOwner,
} from "../actions/storeActions";

class StoreOwnerEntry extends Component {
  state = {};

  handleRemoveOwner = () => {
    if (this.props.isAuthenticated) {
      this.props
        .dispatch(removeStoreOwner(this.props.myRole?.guid, this.props.guid))
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
        {this.props.isSubordinate ? (
          <td>
            <button
              onClick={this.handleRemoveOwner}
              className="d-flex p-1 justify-content-center btn btn-outline-primary"
            >
              {" "}
              Remove Owner{" "}
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
export default connect(mapStateToProps)(StoreOwnerEntry);
