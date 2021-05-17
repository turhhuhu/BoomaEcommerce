import React, { Component } from "react";
import { connect } from "react-redux";
import Dialog from "@material-ui/core/Dialog";
import DialogActions from "@material-ui/core/DialogActions";
import DialogContent from "@material-ui/core/DialogContent";
import DialogContentText from "@material-ui/core/DialogContentText";
import DialogTitle from "@material-ui/core/DialogTitle";
import { addStore, fetchUserRoles } from "../actions/userActions";
import { Alert } from "@material-ui/lab";

class AddStoreDialog extends Component {
  state = {
    error: undefined,
    name: "",
    description: "",
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
    if (!this.state.name || !this.state.description) {
      this.setState({ error: "Please fill all fields" });
      return;
    } else {
      event.preventDefault();
      this.setState({ error: undefined });
      this.props
        .dispatch(
          addStore({
            storeName: this.state.name,
            description: this.state.description,
          })
        )
        .then((success) => {
          if (success) {
            this.props.closeDialog();
            this.props.dispatch(fetchUserRoles());
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
        <DialogTitle id="form-dialog-title">Add product</DialogTitle>
        <form>
          <DialogContent>
            <DialogContentText>
              Please fill out the following store details:
            </DialogContentText>
            <label>Name:</label>
            <input
              type="text"
              className="form-control mb-2"
              name="name"
              required
              value={this.state.name}
              onChange={this.handleChange}
            ></input>
            <label>Description:</label>
            <input
              type="text"
              className="form-control mb-2"
              name="description"
              required
              value={this.state.description}
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
  };
};

export default connect(mapStateToProps)(AddStoreDialog);
