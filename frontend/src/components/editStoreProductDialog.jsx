import React, { Component } from "react";
import { connect } from "react-redux";
import Dialog from "@material-ui/core/Dialog";
import DialogActions from "@material-ui/core/DialogActions";
import DialogContent from "@material-ui/core/DialogContent";
import DialogContentText from "@material-ui/core/DialogContentText";
import DialogTitle from "@material-ui/core/DialogTitle";
import { editStoreProduct } from "../actions/storeActions";
import { Alert } from "@material-ui/lab";

class EditStoreProductDialog extends Component {
  state = {
    isDialogOpen: false,
    error: undefined,
    name: "",
    category: "",
    amount: "",
    price: "",
  };

  static getDerivedStateFromProps(nextProps, prevProps) {
    if (prevProps.isDialogOpen !== nextProps.isDialogOpen) {
      return { isDialogOpen: nextProps.isDialogOpen };
    }
    return null;
  }

  handleChange = (event) => {
    this.setState({
      [event.target.name]: event.target.value,
    });
  };

  handleClose = (event) => {
    event.preventDefault();
    this.setState({ isDialogOpen: false });
    this.props.closeDialog();
  };

  handleSubmit = (event) => {
    if (
      !this.state.name &&
      !this.state.category &&
      !this.state.amount &&
      !this.state.price
    ) {
      this.setState({ error: "Please fill at least one field to edit" });
      return;
    } else {
      this.setState({ error: undefined });
      event.preventDefault();
      this.props
        .dispatch(
          editStoreProduct(this.props.storeGuid, this.props.guid, {
            name: this.state.name ? this.state.name : undefined,
            category: this.state.category ? this.state.category : undefined,
            amount: this.state.amount ? this.state.amount : undefined,
            price: this.state.price ? this.state.price : undefined,
          })
        )
        .then((success) => {
          if (success) {
            this.setState({ isDialogOpen: false });
            this.props.closeDialog();
          }
        });
    }
  };

  render() {
    return (
      <Dialog
        open={this.state.isDialogOpen}
        onClose={this.handleClose}
        aria-labelledby="form-dialog-title"
      >
        <DialogTitle id="form-dialog-title">Edit product</DialogTitle>
        <form>
          <DialogContent>
            <DialogContentText>
              Please fill out the following product details:
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
            <label>category:</label>
            <input
              type="text"
              className="form-control mb-2"
              name="category"
              required
              value={this.state.category}
              onChange={this.handleChange}
            ></input>
            <label>amount:</label>
            <input
              type="text"
              className="form-control mb-2"
              name="amount"
              required
              value={this.state.amount}
              onChange={this.handleChange}
            ></input>
            <label>price:</label>
            <input
              type="text"
              className="form-control mb-2"
              name="price"
              required
              value={this.state.price}
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
              Save
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

export default connect(mapStateToProps)(EditStoreProductDialog);
