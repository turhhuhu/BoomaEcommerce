import React, { Component } from "react";
import { connect } from "react-redux";
import TreeView from "@material-ui/lab/TreeView";
import ExpandMoreIcon from "@material-ui/icons/ExpandMore";
import ChevronRightIcon from "@material-ui/icons/ChevronRight";
import TreeItem from "@material-ui/lab/TreeItem";
import {
  fetchStoreDiscounts,
  removeStoreDiscount,
} from "../actions/storeActions";
import AddStoreDiscountDialog from "./addStoreDiscountDialog";
import CallSplitIcon from "@material-ui/icons/CallSplit";

const mapDiscountTypeToLabel = {
  category: "Category discount",
  product: "Product discount",
  basket: "Basket discount",
};

const mapOperatorToLabel = {
  sum: "Sum of the following discounts:",
  max: "Maximum of the following discounts:",
};

class DiscountsTree extends Component {
  state = {
    isDialogOpen: false,
    fatherPolicyGuid: undefined,
  };

  closeDialog = () => {
    this.setState({ isDialogOpen: false });
  };

  handleAddDiscount = (fatherDiscountGuid) => {
    this.setState({
      isDialogOpen: true,
      fatherDiscountGuid: fatherDiscountGuid,
    });
  };

  handleRemoveDiscount = (discountGuid) => {
    this.props
      .dispatch(removeStoreDiscount(this.props.storeGuid, discountGuid))
      .then(fetchStoreDiscounts(this.props.storeGuid));
  };

  createDiscountTree = (discount) => {
    if (discount.type === "composite") {
      return (
        <TreeItem
          key={discount.guid}
          label={discount.type}
          nodeId={discount.guid}
        >
          <button
            onClick={() => this.handleAddDiscount(discount.guid)}
            className="btn btn-outline-primary btn-sm mb-2 mt-2"
          >
            {" "}
            Add sub-discount
          </button>
          <button
            onClick={() => this.handleRemoveDiscount(discount.guid)}
            className="btn btn-outline-secondary btn-sm mb-2 mt-2 ml-2"
          >
            {" "}
            Remove discount
          </button>
          <TreeItem
            icon={<CallSplitIcon />}
            label={mapOperatorToLabel[discount.operator]}
            nodeId={discount.guid + "1"}
            key={discount.guid + "1"}
          />
          {discount.discounts.map((subDiscount) =>
            this.createDiscountTree(subDiscount)
          )}
        </TreeItem>
      );
    } else {
      return (
        <TreeItem
          label={mapDiscountTypeToLabel[discount.type]}
          nodeId={discount.guid}
          key={discount.guid}
        >
          <button
            onClick={() => this.handleRemoveDiscount(discount.guid)}
            className="btn btn-outline-secondary btn-sm mb-2 mt-2"
          >
            {" "}
            Remove discount
          </button>
          <a
            href={`/store/${this.props.storeGuid}/discounts/${discount.guid}`}
            className="btn btn-outline-secondary btn-sm mb-2 mt-2 ml-2"
          >
            {" "}
            Show discount policy
          </a>
          {discount.type === "product" ? (
            <TreeItem
              label={`Product name: 
                ${
                  this.props.storeProducts.find(
                    (product) => product.guid === discount.productGuid
                  )?.name
                }`}
              nodeId={discount.guid + "1"}
              key={discount.guid + "1"}
            ></TreeItem>
          ) : null}
          {discount.type === "category" ? (
            <TreeItem
              label={`Category: 
                ${discount.category}`}
              nodeId={discount.guid + "1"}
              key={discount.guid + "1"}
            ></TreeItem>
          ) : null}
          <TreeItem
            label={`Percentage: 
              ${discount.percentage}
            `}
            nodeId={discount.guid + "2"}
            key={discount.guid + "2"}
          />
        </TreeItem>
      );
    }
  };

  render() {
    return (
      <main className="col">
        {this.props.storePolicy?.type ? (
          <React.Fragment>
            <TreeView
              defaultCollapseIcon={<ExpandMoreIcon />}
              defaultExpandIcon={<ChevronRightIcon />}
            >
              {this.createDiscountTree(this.props.storeDiscount)}
            </TreeView>
            <AddStoreDiscountDialog
              fatherDiscountGuid={this.state.fatherDiscountGuid}
              storeGuid={this.props.storeGuid}
              isDialogOpen={this.state.isDialogOpen}
              closeDialog={this.closeDialog}
            />{" "}
          </React.Fragment>
        ) : null}
      </main>
    );
  }
}
const mapStateToProps = (store) => {
  return {
    storeDiscount: store.store.storeDiscount,
    storeProducts: store.store.products,
  };
};
export default connect(mapStateToProps)(DiscountsTree);
