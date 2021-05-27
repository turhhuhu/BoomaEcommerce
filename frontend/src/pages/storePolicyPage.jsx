import React, { Component } from "react";
import { connect } from "react-redux";
import { fetchUserStoreRole } from "../actions/userActions";
import { fetchStorePolicy } from "../actions/storeActions";
import Header from "../components/header";
import StorePolicyHeader from "../components/storePolicyHeader";

class StorePolicyPage extends Component {
  state = {};

  componentDidMount() {
    if (this.props.match.params.guid) {
      this.props.dispatch(fetchUserStoreRole(this.props.match.params.guid));
      this.props.dispatch(fetchStorePolicy(this.props.match.params.guid));
    }
  }
  render() {
    return (
      <React.Fragment>
        <Header />
        <div className="container" style={{ maxWidth: "1200px" }}>
          <section className="section-conten padding-y">
            <StorePolicyHeader
              storeName={this.props.storeInfo?.storeName}
              storeGuid={this.props.match.params.guid}
            />
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
