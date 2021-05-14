import React, { Component } from "react";
import { connect } from "react-redux";
import Rating from "./rating";
import { removeStoreProduct } from "../actions/storeActions";
import EditStoreProductDialog from "./editStoreProductDialog";

class StoreProduct extends Component {
  state = { isDialogOpen: false };

  closeDialog = () => {
    this.setState({ isDialogOpen: false });
  };

  handleEditProduct = () => {
    this.setState({ isDialogOpen: true });
  };

  handleDeleteProduct = () => {
    this.props.dispatch(
      removeStoreProduct(this.props.storeGuid, this.props.guid)
    );
  };

  render() {
    return (
      <div className="col-md-4">
        <figure className="card card-product-grid">
          <figcaption className="info-wrap">
            <div className="fix-height">
              <h6 className="title product-card-item">{this.props.name}</h6>
              <p className="text-muted small product-card-item">
                Amount in store: {this.props.amount}
                <br />
                Category: {this.props.category}
              </p>
              <div className="product-card-item">
                <Rating rating={this.props.rating} />
              </div>
              <div className="price-wrap mt-2 product-card-item">
                <label>Price per unit:</label>
                <span className="price"> {this.props.price}</span>
              </div>
            </div>
            <button
              onClick={this.handleEditProduct}
              className="btn btn-outline-primary btn-block"
            >
              {" "}
              Edit Product
              <i className="ml-2 fa fa-edit"></i>
            </button>
            <EditStoreProductDialog
              isDialogOpen={this.state.isDialogOpen}
              closeDialog={this.closeDialog}
              guid={this.props.guid}
              storeGuid={this.props.storeGuid}
            />
            <button
              onClick={this.handleDeleteProduct}
              className="btn btn-outline-primary btn-block"
            >
              {" "}
              Remove Product
              <i className="ml-2 fa fa-trash"></i>
            </button>
          </figcaption>
        </figure>
      </div>
    );
  }
}

export default connect()(StoreProduct);
