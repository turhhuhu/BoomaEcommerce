import React, { Component } from "react";
import { connect } from "react-redux";
import Dialog from "@material-ui/core/Dialog";
import DialogActions from "@material-ui/core/DialogActions";
import DialogContent from "@material-ui/core/DialogContent";
import DialogContentText from "@material-ui/core/DialogContentText";
import DialogTitle from "@material-ui/core/DialogTitle";
import { Alert } from "@material-ui/lab";
import {
  addStoreOwner,
  fetchStoreRoles,
  fetchStoreSubordinates,
} from "../actions/storeActions";

class AddStoreOwnerDialog extends Component {
  state = {
    error: undefined,
    nominatedUserName: "",
  };

  handleChange = (event) => {
    this.setState({
      [event.target.name]: event.target.value,
    });
  };

  handleClose = (event) => {
    event.preventDefault();
    this.props.closeDialog();
  };

  handleSubmit = (event) => {
    if (!this.state.nominatedUserName) {
      this.setState({ error: "Please fill all fields" });
      return;
    } else {
      event.preventDefault();
      this.setState({ error: undefined });
      this.props
        .dispatch(
          addStoreOwner(
            {
              nominatedUserName: this.state.nominatedUserName,
              nominatingOwnershipGuid: this.props.myRole?.guid,
            },
            this.props.storeGuid
          )
        )
        .then((success) => {
          if (success) {
            this.props.closeDialog();
            this.props.dispatch(fetchStoreRoles(this.props.storeGuid));
            if (this.props.myRole?.type === "ownership") {
              this.props.dispatch(
                fetchStoreSubordinates(
                  this.props.storeGuid,
                  this.props.myRole.guid
                )
              );
            }
          }
        });
    }
  };
  render() {
    return (
      <Dialog
        open={this.props.isDialogOpen}
        onClose={this.handleClose}
        aria-labelledby="form-dialog-title"
      >
        <DialogTitle id="form-dialog-title">Add Owner</DialogTitle>
        <form>
          <DialogContent>
            <DialogContentText>
              Please fill out the following username for the new owner:
            </DialogContentText>
            <label>Username:</label>
            <input
              type="text"
              className="form-control mb-2"
              name="nominatedUserName"
              required
              value={this.state.nominatedUserName}
              onChange={this.handleChange}
            ></input>
          </DialogContent>
          <DialogActions>
            <button
              className="btn btn-outline-primary my-2"
              onClick={this.handleClose}
            >
              Cancel
            </button>
            <button
              className="btn btn-outline-primary my-2"
              onClick={this.handleSubmit}
            >
              Add
            </button>
          </DialogActions>
          {(this.props.error || this.state.error) && (
            <Alert severity="error" onClick={() => this.setState(null)}>
              {this.props.error || this.state.error}
            </Alert>
          )}
        </form>
      </Dialog>
    );
  }
}

const mapStateToProps = (store) => {
  return {
    error: store.store.error,
    myRole: store.user.storeRole,
  };
};

export default connect(mapStateToProps)(AddStoreOwnerDialog);
