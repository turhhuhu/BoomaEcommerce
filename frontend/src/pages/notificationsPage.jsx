import React, { Component } from "react";
import { connect } from "react-redux";
import { fetchUserInfo } from "../actions/userActions";
import Header from "../components/header";
import Notifications from "../components/notifications";

class NotificationsPage extends Component {
  componentDidMount() {
    this.props.dispatch(fetchUserInfo());
  }

  state = {};
  render() {
    return (
      <React.Fragment>
        <Header />
        <div className="container" style={{ maxWidth: "1000px" }}>
          <section className="section-conten padding-y">
            <Notifications guid={this.props.match.params.guid} />
          </section>
        </div>
      </React.Fragment>
    );
  }
}

export default connect()(NotificationsPage);
