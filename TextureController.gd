extends Node
var my_csharp_node

func swap(image: Image):
	if get_owner() != null:
		var Parent = get_tree().get_root().get_node("Renderer")
		Parent.texture = ImageTexture.create_from_image(image)
	pass
