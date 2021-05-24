import React, { Component } from "react";
import { connect } from "react-redux";
import { receiveRegularNotification } from "../actions/userActions";
import { setupSignalRConnection } from "../signalR";
import { NOTIFICATION_HUB_URL } from "../utils/constants";
import Snackbar from "@material-ui/core/Snackbar";
import { Alert } from "@material-ui/lab";
import Slide from "@material-ui/core/Slide";
import NotificationsIcon from "@material-ui/icons/Notifications";

function SlideTransition(props) {
  return <Slide {...props} direction="up" />;
}

class NotificationIcon extends Component {
  state = { isSnackBarOpen: false, snackBarMessage: "" };

  componentDidMount() {
    if (this.props.isAuthenticated) {
      const setupEventsHub = setupSignalRConnection(NOTIFICATION_HUB_URL, {
        notification: (notification) => {
          this.setState({
            isSnackBarOpen: true,
            snackBarMessage: notification?.message,
          });
          return receiveRegularNotification(notification);
        },
      });
      this.props.dispatch(setupEventsHub);
    }
  }

  handleClose = () => {
    this.setState({ isSnackBarOpen: false });
  };

  handleOpen = () => {
    this.setState({ isSnackBarOpen: true });
  };

  render() {
    return (
      <div className="widget-header  mr-3">
        <a
          href="/user/notifications"
          className="icon icon-sm rounded-circle border"
        >
          <i className="fa fa-bell white"></i>
        </a>
        <span className="badge badge-pill badge-secondary notify">
          {
            this.props.notifications?.filter(
              (notification) => notification.wasSeen === false
            ).length
          }
        </span>
        <Snackbar
          anchorOrigin={{ vertical: "bottom", horizontal: "left" }}
          open={this.state.isSnackBarOpen}
          message={this.state.snackBarMessage}
          TransitionComponent={SlideTransition}
          autoHideDuration={6000}
          onClose={this.handleClose}
        >
          <Alert
            icon={<NotificationsIcon />}
            onClose={this.handleClose}
            severity="info"
            variant="outlined"
          >
            {this.state.snackBarMessage}
          </Alert>
        </Snackbar>
      </div>
    );
  }
}

const mapStatetoProps = (store) => {
  return {
    notifications: store.user.notifications,
    webSocketConnection: store.user.webSocketConnection,
    isAuthenticated: store.auth.isAuthenticated,
  };
};

export default connect(mapStatetoProps)(NotificationIcon);
