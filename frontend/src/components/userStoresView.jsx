import React, { Component } from "react";
import { connect } from "react-redux";
import UserStoreTable from "./userStoreTable";
import ProfileSideBar from "./profileSideBar";

class UserStoresView extends Component {
  state = {};

  render() {
    return (
      <div className="row mt-3">
        <ProfileSideBar isStores="true" />
        <main className="col-md-6">
          <UserStoreTable
            title="Founder Stores"
            stores={this.props.userRoles?.ownerFounderRoles?.map(
              (role) => role.storeMetaData
            )}
          />
          <UserStoreTable
            title="Owner Stores"
            stores={this.props.userRoles?.ownerNotFounderRoles?.map(
              (role) => role.storeMetaData
            )}
          />
          <UserStoreTable
            title="Manager Stores"
            stores={this.props.userRoles?.managerRoles?.map(
              (role) => role.storeMetaData
            )}
          />
        </main>
      </div>
    );
  }
}

export default connect()(UserStoresView);
