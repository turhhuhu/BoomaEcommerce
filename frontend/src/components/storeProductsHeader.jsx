import React, { Component } from "react";
import { connect } from "react-redux";
import Dialog from "@material-ui/core/Dialog";
import DialogActions from "@material-ui/core/DialogActions";
import DialogContent from "@material-ui/core/DialogContent";
import DialogContentText from "@material-ui/core/DialogContentText";
import DialogTitle from "@material-ui/core/DialogTitle";
import { addProductToStore } from "../actions/storeActions";
import { Alert } from "@material-ui/lab";

class StoreProductsHeader extends Component {
  state = {
    isDialogOpen: false,
    name: "",
    category: "",
    amount: "",
    price: "",
  };

  handleChange = (event) => {
    this.setState({
      [event.target.name]: event.target.value,
    });
  };

  handleOpen = () => {
    this.setState({ isDialogOpen: true });
  };
  handleClose = (event) => {
    event.preventDefault();
    this.setState({ isDialogOpen: false });
  };
  handleSubmit = (event) => {
    if (
      !this.state.name ||
      !this.state.category ||
      !this.state.amount ||
      !this.state.price
    ) {
      return;
    } else {
      event.preventDefault();
      this.props
        .dispatch(
          addProductToStore({
            name: this.state.name,
            category: this.state.category,
            amount: this.state.amount,
            price: this.state.price,
            storeGuid: this.props.guid,
          })
        )
        .then((success) => {
          if (success) {
            this.setState({ isDialogOpen: false });
            this.props.refreshStoreProducts();
          }
        });
    }
  };
  render() {
    return (
      <div className="container" style={{ maxWidth: "1400px" }}>
        <section className="text-center border-bottom">
          <h1 className="jumbotron-heading">{`${this.props.storeName} Products`}</h1>
          <p>
            <button
              onClick={this.handleOpen}
              className="btn btn-outline-primary my-2"
            >
              Add Product
            </button>
            <Dialog
              open={this.state.isDialogOpen}
              onClose={this.handleClose}
              aria-labelledby="form-dialog-title"
            >
              <DialogTitle id="form-dialog-title">Add product</DialogTitle>
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
          </p>
        </section>
      </div>
    );
  }
}

const mapStateToProps = (store) => {
  return {
    error: store.store.error,
  };
};

export default connect(mapStateToProps)(StoreProductsHeader);
