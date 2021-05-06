import React, { Component } from "react";
import { connect } from "react-redux";
import UserStore from "./userStore";
import ProfileSideBar from "./profileSideBar";

class UserStoresView extends Component {
  state = {};
  render() {
    return (
      <div className="row">
        <ProfileSideBar isStores="true" />
        <main className="col-md-6">
          <UserStore />
          {/* <div className="row">
            {this.props.founderStores?.map((store) => (
              <UserStore key={store.guid} />
            ))}
          </div>
          <div className="row">
            {this.props.founderStores?.map((store) => (
              <UserStore key={store.guid} />
            ))}
          </div>
          <div className="row">
            {this.props.founderStores?.map((store) => (
              <UserStore key={store.guid} />
            ))}
          </div> */}
        </main>
      </div>
    );
  }
}

export default connect()(UserStoresView);
