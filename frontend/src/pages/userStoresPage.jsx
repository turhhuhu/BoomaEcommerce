import React, { Component } from "react";
import { connect } from "react-redux";
import UserStoresView from "../components/userStoresView";
import Header from "../components/header";
import { Redirect } from "react-router";

class ProfileStoresPage extends Component {
  state = {};
  render() {
    if (!this.props.isAuthenticated) {
      return <Redirect to="/login" />;
    }
    return (
      <React.Fragment>
        <Header />
        <div className="container" style={{ maxWidth: "1000px" }}>
          <section className="section-conten padding-y">
            <UserStoresView />
          </section>
        </div>
      </React.Fragment>
    );
  }
}

const mapStateToProps = (store) => {
  return {
    isAuthenticated: store.auth.isAuthenticated,
  };
};

export default connect(mapStateToProps)(ProfileStoresPage);
