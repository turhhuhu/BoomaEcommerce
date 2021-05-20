import React, { Component } from "react";
import { connect } from "react-redux";
import StoreManagersTable from "./storeManagersTable";
import StoreOwnersTable from "./storeOwnersTable";
import StoreSideBar from "./storeSideBar";

class StoreManagementView extends Component {
  state = {};

  render() {
    return (
      <div className="row mt-3">
        <StoreSideBar isManagement="true" guid={this.props.guid} />
        <main className="row col">
          <div>
            <StoreManagersTable
              title="Managers"
              myRole={this.props.myRole}
              subordinates={this.props.subordinates?.storeManagers}
              managers={this.props.storeRoles?.storeManagers}
            />
          </div>
          <div>
            <StoreOwnersTable
              title="Owners"
              myRole={this.props.myRole}
              subordinates={this.props.subordinates?.storeOwners}
              owners={this.props.storeRoles?.storeOwners}
            />
          </div>
        </main>
      </div>
    );
  }
}

const mapStateToProps = (store) => {
  return {
    myRole: store.user.storeRole,
    storeRoles: store.store.storeRoles,
    subordinates: store.store.subordinates,
  };
};

export default connect(mapStateToProps)(StoreManagementView);
