import React, { Component } from "react";
import { connect } from "react-redux";
import Dialog from "@material-ui/core/Dialog";
import DialogActions from "@material-ui/core/DialogActions";
import DialogContent from "@material-ui/core/DialogContent";
import DialogContentText from "@material-ui/core/DialogContentText";
import DialogTitle from "@material-ui/core/DialogTitle";
import { Checkbox } from "@material-ui/core";
import { Alert } from "@material-ui/lab";

class AddStoreOwnerDialog extends Component {
  state = {
    error: undefined,
    nominatedUserName: "",
    canAddProduct: false,
    canDeleteProduct: false,
    canUpdateProduct: false,
    canGetSellersInfo: false,
  };

  handleChange = (event, isCheckBox) => {
    if (isCheckBox) {
      this.setState({
        [event.target.name]: event.target.checked,
      });
    } else {
      this.setState({
        [event.target.name]: event.target.value,
      });
    }
  };

  handleClose = (event) => {
    event.preventDefault();
    this.props.closeDialog();
  };

  handleSubmit = (event) => {
    if (!this.state.nominatedUserName) {
      this.setState({ error: "Username for the new manager is required" });
      return;
    } else {
      event.preventDefault();
      this.setState({ error: undefined });
      // this.props
      //   .dispatch(
      //     addStoreManager({
      //       nominatedUserName: this.state.nominatedUserName,
      //       canAddProduct: this.state.canAddProduct,
      //       canDeleteProduct: this.state.canDeleteProduct,
      //       canUpdateProduct: this.state.canUpdateProduct,
      //       canGetSellersInfo: this.state.facanGetSellersInfolse,
      //     })
      //   )
      //   .then((success) => {
      //     if (success) {
      //       this.props.closeDialog();
      //       //TODO: add fetch for other sellers
      //     }
      //   });
    }
  };
  render() {
    return (
      <Dialog
        open={this.props.isDialogOpen}
        onClose={this.handleClose}
        aria-labelledby="form-dialog-title"
      >
        <DialogTitle id="form-dialog-title">Add Manager</DialogTitle>
        <form>
          <DialogContent>
            <DialogContentText>
              Please fill out the following details for the new manager:
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
            <div>
              <label>Can add product:</label>
              <Checkbox
                color="primary"
                name="canAddProduct"
                onChange={(event) => this.handleChange(event, true)}
                checked={this.state.canAddProduct}
              />
            </div>
            <div>
              <label>Can remove product:</label>
              <Checkbox
                color="primary"
                name="canDeleteProduct"
                onChange={(event) => this.handleChange(event, true)}
                checked={this.state.canDeleteProduct}
              />
            </div>
            <div>
              <label>Can edit product:</label>
              <Checkbox
                color="primary"
                name="canUpdateProduct"
                onChange={(event) => this.handleChange(event, true)}
                checked={this.state.canUpdateProduct}
              />
            </div>
            <div>
              <label>Can view other managers/owners:</label>
              <Checkbox
                color="primary"
                name="canGetSellersInfo"
                onChange={(event) => this.handleChange(event, true)}
                checked={this.state.canGetSellersInfo}
              />
            </div>
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

export default connect(mapStateToProps)(AddStoreOwnerDialog);
