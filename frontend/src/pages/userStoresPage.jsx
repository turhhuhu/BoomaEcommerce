import React, { Component } from "react";
import { connect } from "react-redux";
import UserStoresView from "../components/userStoresView";
import Header from "../components/header";
import { Redirect } from "react-router";
import UserStoresHeader from "../components/userStoresHeader";
import { fetchUserRoles } from "../actions/userActions";

class ProfileStoresPage extends Component {
  state = {};

  componentDidMount() {
    if (this.props.isAuthenticated) {
      this.props.dispatch(fetchUserRoles());
    }
  }

  render() {
    if (!this.props.isAuthenticated) {
      return <Redirect to="/login" />;
    }
    return (
      <React.Fragment>
        <Header />
        <div className="container" style={{ maxWidth: "1000px" }}>
          <section className="section-conten padding-y">
            <UserStoresHeader username={this.props.username} />
            <UserStoresView
              userRoles={this.props.userRoles}
              isAuthenticated={this.props.isAuthenticated}
            />
          </section>
        </div>
      </React.Fragment>
    );
  }
}

const mapStateToProps = (store) => {
  return {
    userRoles: store.user.userRoles,
    isAuthenticated: store.auth.isAuthenticated,
    username: store.auth.username,
  };
};

export default connect(mapStateToProps)(ProfileStoresPage);
