import React, { Component } from "react";
import { connect } from "react-redux";
import StoreManagementTable from "./storeManagersTable";
import StoreSideBar from "./storeSideBar";

class StoreManagementView extends Component {
  state = {};

  render() {
    return (
      <div className="row mt-3">
        <StoreSideBar isManagement="true" />
        <main className="col-md-6">
          <StoreManagementTable
            title="Owners"
            stores={this.props.userRoles?.ownerFounderRoles?.map(
              (role) => role.storeMetaData
            )}
          />
          <StoreManagementTable
            title="Managers"
            stores={this.props.userRoles?.ownerNotFounderRoles?.map(
              (role) => role.storeMetaData
            )}
          />
        </main>
      </div>
    );
  }
}

export default connect()(StoreManagementView);
