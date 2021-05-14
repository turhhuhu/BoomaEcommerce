import React, { Component } from "react";
import { connect } from "react-redux";
import UserStoreTable from "./userStoreTable";
import ProfileSideBar from "./profileSideBar";
import { fetchUserRoles } from "../actions/userActions";

class UserStoresView extends Component {
  state = {};

  componentDidMount() {
    if (this.props.isAuthenticated) {
      this.props.dispatch(fetchUserRoles());
    }
  }

  render() {
    return (
      <div className="row">
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

const mapStateToProps = (store) => {
  return {
    userRoles: store.user.userRoles,
    isAuthenticated: store.auth.isAuthenticated,
  };
};

export default connect(mapStateToProps)(UserStoresView);
