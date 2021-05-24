import React, { Component } from "react";
import { connect } from "react-redux";
import NotificationEntry from "./notificationEntry";

class Notifcations extends Component {
  state = {};
  render() {
    return (
      <div>
        <article className="card mb-3">
          <header
            className="card-header pl-3"
            style={{ backgroundColor: "rgba(69, 136, 236, 0.125)" }}
          >
            <h5>Notifications</h5>
          </header>
          <div
            className="table-responsive "
            style={{
              display: "block",
              maxHeight: "700px",
              overflowY: "scroll",
            }}
          >
            <table className="table table-hover table-bordered">
              <tbody>
                <tr className="table-primary">
                  <th scope="col">Type:</th>
                  <th scope="col">Message:</th>
                </tr>
                {this.props.notifications?.map((notification) => (
                  <NotificationEntry
                    key={notification.guid}
                    guid={notification.guid}
                    type={notification.type}
                    wasSeen={notification.wasSeen}
                    message={notification.message}
                  />
                ))}
              </tbody>
            </table>
          </div>
        </article>
      </div>
    );
  }
}

const mapStateToProps = (store) => {
  return {
    notifications: store.user.notifications,
  };
};

export default connect(mapStateToProps)(Notifcations);
