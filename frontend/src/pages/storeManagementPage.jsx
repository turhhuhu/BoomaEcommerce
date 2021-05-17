import React, { Component } from "react";
import { connect } from "react-redux";

class StoreManagement extends Component {
  state = {};
  render() {
    return (
      <React.Fragment>
        <Header />
        <div className="container" style={{ maxWidth: "1000px" }}>
          <section className="section-conten padding-y">
            <StoreManagementHeader storeName={this.props.storeName} />
            <StoreManagementView />
          </section>
        </div>
      </React.Fragment>
    );
  }
}

export default StoreManagement;
