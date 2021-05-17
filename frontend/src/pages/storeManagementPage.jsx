import React, { Component } from "react";
import { connect } from "react-redux";
import Header from "../components/header";
import {
  fetchStoreInfo,
  fetchStoreRoles,
  fetchStoreSubordinates,
} from "../actions/storeActions";
import StoreManagementHeader from "../components/storeManagementHeader";
import StoreManagementView from "../components/storeManagementView";
import { fetchUserStoreRole } from "../actions/userActions";

class StoreManagement extends Component {
  state = {};

  componentDidMount(prevProps) {
    if (this.props !== prevProps) {
      this.props.dispatch(fetchStoreInfo(this.props.match.params.guid));
      this.props
        .dispatch(fetchUserStoreRole(this.props.match.params.guid))
        .then((action) => {
          if (action.payload.response.type === "ownership") {
            this.props.dispatch(
              fetchStoreSubordinates(
                this.props.match.params.guid,
                action.payload.response.guid
              )
            );
          }
        });
      this.props.dispatch(fetchStoreRoles(this.props.match.params.guid));
    }
  }

  render() {
    return (
      <React.Fragment>
        <Header />
        <div className="container" style={{ maxWidth: "1200px" }}>
          <section className="section-conten padding-y">
            <StoreManagementHeader storeName={this.props.storeInfo.storeName} />
            <StoreManagementView />
          </section>
        </div>
      </React.Fragment>
    );
  }
}

const mapStateToProps = (store) => {
  return {
    storeInfo: store.store.storeInfo,
  };
};

export default connect(mapStateToProps)(StoreManagement);
