import React, { Component } from "react";
import { connect } from "react-redux";
import { fetchUserStoreRole } from "../actions/userActions";
import {
  fetchAllStoreProducts,
  fetchStorePolicy,
} from "../actions/storeActions";
import Header from "../components/header";
import StorePolicyHeader from "../components/storePolicyHeader";
import PolicyTree from "../components/policyTree";
import StoreSideBar from "../components/storeSideBar";

class StorePolicyPage extends Component {
  state = {};

  componentDidMount() {
    if (this.props.match.params.guid) {
      this.props.dispatch(fetchUserStoreRole(this.props.match.params.guid));
      this.props.dispatch(fetchStorePolicy(this.props.match.params.guid));
      this.props.dispatch(fetchAllStoreProducts(this.props.match.params.guid));
    }
  }
  render() {
    return (
      <React.Fragment>
        <Header />
        <div className="container" style={{ maxWidth: "1000px" }}>
          <section className="section-conten padding-y">
            <StorePolicyHeader
              storeName={this.props.storeInfo?.storeName}
              storeGuid={this.props.match.params.guid}
            />
            <div className="row mt-2">
              <StoreSideBar
                isPolicy="true"
                guid={this.props.match.params.guid}
                colClass="col-3"
              />
              <PolicyTree storeGuid={this.props.match.params.guid} />
            </div>
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

export default connect(mapStateToProps)(StorePolicyPage);
